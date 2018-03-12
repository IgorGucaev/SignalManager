using EventsStore.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsStore.Core
{
    public class JsonMessageSerializer<T> : IMessageSerializer<T>
    {
        public T Deserialize(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public string Serialize(T item)
        {
            return JsonConvert.SerializeObject(item, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            //"yyyy-MM-dd HH:mm:ss \"GMT\"zzz"
        }
    }
}
