using EventsManager.LocalEventStorage.Abstractions;
using EventsStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteAdapter
{
    public class SQLiteCacheSettings : ICacheSettings
    {
        public enum CommandType
        {
            Select,
            CreateIfNotExists,
            Delete,
            Insert,
            Truncate
        };

        public string Database { get; private set; }
        public string TableName { get; private set; }
        public string DateColumnName { get; private set; }

        public string this[CommandType queryType]
        {
            get { return this.QueryTable[queryType]; }
        }

        private Dictionary<CommandType, string> QueryTable = new Dictionary<CommandType, string>();

        public SQLiteCacheSettings(string database, string table, string dateColumn)
        {
            if (String.IsNullOrWhiteSpace(database))
                throw new ArgumentException("Database not specified");

            this.Database = database;

            if (String.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Table not specified");

            this.TableName = table;

            if (String.IsNullOrWhiteSpace(dateColumn))
                throw new ArgumentException("Date column not specified");

            this.DateColumnName = dateColumn;
        }

        public SQLiteCacheSettings AddQuery(CommandType queryType, string queryValue)
        {
            if (String.IsNullOrWhiteSpace(queryValue))
                throw new ArgumentException("Database not specified");

            this.QueryTable[queryType] = queryValue;

            return this;
        }

        public SQLiteCacheSettings AddDefaultQueries()
        {
            if (String.IsNullOrWhiteSpace(TableName))
                throw new ArgumentException("Table name must be specified");

            this.QueryTable = DefaultQueries.GetDefaultQueries(TableName);

            return this;
        }

        public string GetValue(CacheVariable key)
        {
            switch (key)
            {
                case CacheVariable.DatabaseName:
                    return this.Database;
                case CacheVariable.TableName:
                    return this.TableName;
                case CacheVariable.DateColumnName:
                    return this.DateColumnName;
                default:
                    return "";
            }
        }
    }
}
