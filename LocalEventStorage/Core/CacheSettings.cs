using EventsManager.LocalEventStorage.Abstractions;
using EventsStore.Abstractions;
using System;
using System.Collections.Generic;

namespace EventsStore.Core
{
    public class CacheSettings : Dictionary<CacheVariable, string>, ICacheSettings
    {
        public string GetValue(CacheVariable key)
        {
            if (!this.ContainsKey(key))
                throw new KeyNotFoundException($"Key {key} does not exist!");

            return this[key];
        }

        new public CacheSettings Add(CacheVariable key, string value)
        {
            if (base.ContainsKey(key))
                this[key] = value;
            else base.Add(key, value);

            return this;
        }
    }
}
