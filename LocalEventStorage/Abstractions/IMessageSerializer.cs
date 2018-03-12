using System;
using System.Collections.Generic;
using System.Text;

namespace EventsStore.Abstractions
{
    public interface IMessageSerializer<T>
    {
        string Serialize(T item);

        T Deserialize(string data);
    }
}
