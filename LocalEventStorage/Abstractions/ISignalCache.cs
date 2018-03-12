using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EventsStore.Abstractions
{
    public interface ISignalCache<T>
    {
        bool Connect();

        void Write(T signal);

        void Write(IEnumerable<T> signals);

        void Write(string serialized);

        void Write(string[] serialized);

        void Truncate();

        IEnumerable<string> GetSignalsSerialized(Func<object[], bool> predicate, Func<object[], object[]> selector);

        void Disconnect();
    }
}
