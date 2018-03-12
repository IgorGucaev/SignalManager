using SQLiteAdapter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EventsManager.LocalEventStorage.Test
{
    public class SQLiteTest
    {
        /// <summary>
        /// Connect to database and create table if not exists
        /// </summary>
        [Fact]
        public void SQLite__Adapter_Connect_Disconnect()
        {
            // Prepare
            SQLiteCacheSettings settings = new SQLiteCacheSettings("fooDatabase", "barTable")
                .AddDefaultQueries();
            SQLiteCacheAdapter adapter = new SQLiteCacheAdapter(settings);

            // Pre-validate
            Assert.True(settings.Database.Length > 0 && settings.TableName.Length > 0, "Database or tablename must be specified!");

            // Perform
            bool connected = adapter.Connect();
            adapter.Disconnect();

            // Post-validate
            Assert.True(connected);
            Assert.True(adapter.Connection.State == System.Data.ConnectionState.Closed);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.GetRandomAntennaSignalRow), MemberType = typeof(TestDataGenerator))]
        public void SQLite__Insert_Rows(string serializedSignal, DateTime timestamp)
        {
            // Prepare
            SQLiteCacheSettings settings = new SQLiteCacheSettings("fooDatabase", "barTable")
               .AddDefaultQueries();
            SQLiteCacheAdapter adapter = new SQLiteCacheAdapter(settings);
            bool connected = adapter.Connect();

            // Pre-validate
            Assert.True(connected);

            // Perform
            adapter.Insert(new object[] { serializedSignal, timestamp });
            var result = adapter.Select();
            adapter.Disconnect();

            // Post-Validate
            Assert.Equal(serializedSignal, result.Last()[1]); // signal inserted correctly
        }

        [Fact]
        public void SQLite__Truncate_Table()
        {
            // Prepare
            SQLiteCacheSettings settings = new SQLiteCacheSettings("fooDatabase", "barTable")
               .AddDefaultQueries();
            SQLiteCacheAdapter adapter = new SQLiteCacheAdapter(settings);
            bool connected = adapter.Connect();
            adapter.Insert(new object[] { "baz", DateTime.Now });

            // Pre-validate
            List<object[]> beforeTruncateData = adapter.Select();
            Assert.NotEmpty(beforeTruncateData);

            // Perform
            adapter.Truncate();

            // Post-validate
            List<object[]> afterTruncateData = adapter.Select();
            Assert.Empty(afterTruncateData);
            adapter.Disconnect();
        }

        [Fact]
        public void SQLite__Insert_Rows__InSingleTransaction_Stress()
        {
            // Prepare
            SQLiteCacheSettings settings = new SQLiteCacheSettings("fooDatabase", "barTable").AddDefaultQueries();
            SQLiteCacheAdapter adapter = new SQLiteCacheAdapter(settings);
            bool connected = adapter.Connect();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            int testSize = 100000;

            // Pre-validate
            Assert.True(connected);
            adapter.Truncate();
            Assert.Empty(adapter.Select());

            // Perform
            object[] dummyRow = new object[] { Guid.NewGuid().ToString(), DateTime.Now };
            List<object[]> rows = new List<object[]>();
            for (int i = 0; i < testSize; i++)
                rows.Add(dummyRow);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            adapter.Insert(rows);
            sw.Stop();


            // Post-validate
            Assert.Equal(testSize, adapter.Select().Count());
            double averageInsertSpeed = testSize / sw.Elapsed.TotalSeconds;
            adapter.Disconnect();
            Assert.True(averageInsertSpeed > 10000);
        }

        [Fact]
        public void SQLite__Insert_Rows__Stress()
        {
            // Prepare
            SQLiteCacheSettings settings = new SQLiteCacheSettings("fooDatabase", "barTable").AddDefaultQueries();
            SQLiteCacheAdapter adapter = new SQLiteCacheAdapter(settings);
            bool connected = adapter.Connect();
            int insertions = 0;
            int testCount = 150000;

            // Pre-validate
            Assert.True(connected);
            adapter.Truncate();
            Assert.Empty(adapter.Select());

            // Perform
            AutoResetEvent arEvent = new AutoResetEvent(false);
            object[] dummyRow = new object[] { Guid.NewGuid().ToString(), DateTime.Now };

            Task t = Task.Factory.StartNew(() =>
            {
                while (insertions < testCount)
                {
                    adapter.InsertAsync(dummyRow);
                    insertions++;
                }

                arEvent.Set();
            });
            arEvent.WaitOne();
            Thread.Sleep(100); // Wait for async save because the insert function is asynchronous
            
            // Post-validate
            int afterInsertRowCount = adapter.Select().Count();
            Assert.Equal(testCount, afterInsertRowCount); adapter.Disconnect();

            adapter.Disconnect();
         }

        [Fact]
        public void SQLite_Delete_Rows_Created_Before_Date()
        {
            // Prepare
            SQLiteCacheSettings settings = new SQLiteCacheSettings("fooDatabase", "barTable").AddDefaultQueries();
            SQLiteCacheAdapter adapter = new SQLiteCacheAdapter(settings);
            bool connected = adapter.Connect();

            // Pre-validate
            Assert.True(connected);
            adapter.Truncate();
            Assert.Empty(adapter.Select());
            DateTime border = DateTime.Now.AddMinutes(-1);
            string topGuid = Guid.NewGuid().ToString();
            adapter.Insert(new List<object[]> {
                new object[] { Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(-3) },
                new object[] { Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(-2) },
                new object[] { topGuid, border }
            });
            Assert.Equal(3, adapter.Select().Count);

            // Perform
            adapter.Delete(border);

            // Post-validate
            List<object[]> data = adapter.Select();
            int afterInsertRowCount = data.Count();
            Assert.Equal(1, afterInsertRowCount);
            Assert.Equal(topGuid, data.First()[1]);
            adapter.Disconnect();
        }
    }
}