using EventsStore.Abstractions;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SQLiteAdapter
{
    /// <summary>
    /// The adapter does not support async operations cause sqlite does not supports concurent transactions
    /// </summary>
    public class SQLiteCacheAdapter
    {
        private SqliteConnection _Connection;
        private SQLiteCacheSettings _Settings;

        private List<object[]> insertBuffer = new List<object[]>();
        private CancellationTokenSource tokenSrc = new CancellationTokenSource();
        private bool asyncInsertion = false;

        public SqliteConnection Connection
        {
            get
            {
                return this._Connection;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageName">storage name</param>
        public SQLiteCacheAdapter(ICacheSettings settings)
        {
            if (!(settings is SQLiteCacheSettings))
                throw new ArgumentException("Wrong cache settings input!");

            this._Settings = (SQLiteCacheSettings)settings;

            Task bulkSave = Task.Factory.StartNew(() =>
            {
                while (!tokenSrc.IsCancellationRequested)
                {
                    lock (insertBuffer)
                    {
                        if (insertBuffer.Any())
                        {
                            asyncInsertion = true;
                            using (SqliteTransaction transaction = _Connection.BeginTransaction())
                            {
                                foreach (var row in insertBuffer)
                                {
                                    SqliteCommand command = _Connection.CreateCommand();
                                    command.CommandText = String.Format(_Settings[SQLiteCacheSettings.CommandType.Insert], row);

                                    int result = command.ExecuteNonQuery(); // The number of rows inserted, updated, or deleted. -1 for SELECT statements.

                                    if (result != 1)
                                        throw new SqliteException($"An error occurred while executing query '{SQLiteCacheSettings.CommandType.Insert}'", (int)SQLiteSpecificErrors.SQLITE_ERROR);
                                }

                                insertBuffer.Clear();

                                transaction.Commit();
                            }
                            asyncInsertion = false;
                        }
                    }

                    Thread.Sleep(50);
                }
            }, tokenSrc.Token);
        }

        private Action<SqliteConnection> checkState = (cntc) =>
        {
            if (cntc.State != System.Data.ConnectionState.Open)
                throw new SqliteException($"Operation unavailable. Not suitable connection state: '{cntc.State}'.", (int)SQLiteSpecificErrors.SQLITE_BADSTATE);
        };

        public bool Connect()
        {
            _Connection = new SqliteConnection($"Filename={_Settings.Database}.db");
            _Connection.Open();

            using (var transaction = _Connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                SqliteCommand command = _Connection.CreateCommand();
                command.CommandText = _Settings[SQLiteCacheSettings.CommandType.CreateIfNotExists];
                command.ExecuteNonQuery();

                transaction.Commit();

                return _Connection.State == System.Data.ConnectionState.Open;
            }
        }

        public void InsertAsync(object[] row)
        {
            checkState(_Connection);
            lock (insertBuffer)
            {
                insertBuffer.Add(row);
            }
        }

        public void Insert(object[] row)
        {
            checkState(_Connection);

            using (var transaction = _Connection.BeginTransaction())
            {
                SqliteCommand command = _Connection.CreateCommand();
                command.CommandText = String.Format(_Settings[SQLiteCacheSettings.CommandType.Insert], row);

                int result = command.ExecuteNonQuery(); // The number of rows inserted, updated, or deleted. -1 for SELECT statements.

                if (result != 1)
                    throw new SqliteException($"An error occurred while executing query '{SQLiteCacheSettings.CommandType.Insert}'", (int)SQLiteSpecificErrors.SQLITE_ERROR);

                transaction.Commit();
            }
        }

        public void Insert(IEnumerable<object[]> rows)
        {
            checkState(_Connection);

            SqliteCommand command = null;
            using (var transaction = _Connection.BeginTransaction())
            {
                foreach (object[] row in rows)
                {
                    command = _Connection.CreateCommand();
                    command.CommandText = String.Format(_Settings[SQLiteCacheSettings.CommandType.Insert], row);

                    int result = command.ExecuteNonQuery(); // The number of rows inserted, updated, or deleted. -1 for SELECT statements.

                    if (result != 1)
                        throw new SqliteException($"An error occurred while executing query '{SQLiteCacheSettings.CommandType.Insert}'", (int)SQLiteSpecificErrors.SQLITE_ERROR);
                }

                transaction.Commit();
            }
        }

        public IEnumerable<object[]> Select(Func<object[], bool> predicate, Func<object[], object[]> selector)
        {
            checkState(_Connection);

            List<object[]> result = new List<object[]>();

            while (asyncInsertion) // Implement timeout
            { Thread.Sleep(50); }

            using (var transaction = _Connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                SqliteCommand command = _Connection.CreateCommand();
                command.CommandText = String.Format(_Settings[SQLiteCacheSettings.CommandType.Select]);
                SqliteDataReader reader = command.ExecuteReader();
                int fieldCount = reader.FieldCount;
                while (reader.Read())
                {
                    object[] row = new object[fieldCount];
                    reader.GetValues(row);
                    result.Add(row);
                }

                transaction.Commit();
            }

            return result.Where(predicate).Select(selector);
        }

        public void Truncate()
        {
            checkState(_Connection);

            using (var transaction = _Connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                SqliteCommand command = _Connection.CreateCommand();
                command.CommandText = String.Format(_Settings[SQLiteCacheSettings.CommandType.Truncate]);
                command.ExecuteNonQuery();

                transaction.Commit();
            }
        }

        public void Delete(DateTime date)
        {
            checkState(_Connection);

            using (var transaction = _Connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                SqliteCommand command = _Connection.CreateCommand();
                command.CommandText = String.Format(_Settings[SQLiteCacheSettings.CommandType.Delete], new object[] { 0, date, 1 });
                command.ExecuteNonQuery();

                transaction.Commit();
            }
        }

        public void Disconnect()
        {
            this.tokenSrc.Cancel();
            this._Connection.Close();
            this._Connection.Dispose();
        }
    }
}
