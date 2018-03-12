using EventsStore.Abstractions;
using Microsoft.Data.Sqlite;
using SQLiteAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EventsManager.LocalEventStorage.Core
{
    public class SQLiteSignalCache<T> : ISignalCache<T>
    {
        private ICacheSettings _Settings;
        private SQLiteCacheAdapter _CacheAdapter;
        private string _Table;
        private string _DateColumnName;
        private IMessageSerializer<T> _Serializer;

        public SQLiteSignalCache(ICacheSettings settings, IMessageSerializer<T> serializer)
        {
            this._Settings = settings;
            this._CacheAdapter = new SQLiteCacheAdapter(settings);

            string address = _Settings.GetValue(Abstractions.CacheVariable.DatabaseName);
            _Serializer = serializer;
            _Table = settings.GetValue( Abstractions.CacheVariable.TableName);
            _DateColumnName = _Settings.GetValue(Abstractions.CacheVariable.DateColumnName);

       //     bool tableExist = CassandraHelper.IsExist(address, _Keyspace, _Table);

         //   if (!tableExist)
                throw new Exception($"Table {settings.GetValue(Abstractions.CacheVariable.TableName)} does not exist on cluster.");

        //    Cluster cluster = Cluster.Builder().AddContactPoint(address).Build();
        //    this._Session = cluster.Connect();

     //       this._Session.CreateKeyspaceIfNotExists(_Keyspace);
        }

        public bool Connect() =>
            this._CacheAdapter.Connect();

        public void Disconnect() =>
            this._CacheAdapter.Disconnect();


        public IEnumerable<string> GetSignalsSerialized(Func<object[], bool> predicate, Func<object[], object[]> select)
        {
            return (IEnumerable<string>)this._CacheAdapter.Select(predicate, select);
        }

        public void Truncate()
        {
            throw new NotImplementedException();
        }

        public void Write(T signal)
        {
            throw new NotImplementedException();
        }

        public void Write(IEnumerable<T> signals)
        {
            throw new NotImplementedException();
        }

        public void Write(string serialized)
        {
            throw new NotImplementedException();
        }

        public void Write(string[] serialized)
        {
            throw new NotImplementedException();
        }
    }
}
