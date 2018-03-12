using EventsManager.LocalEventStorage.Abstractions;
using System;

namespace EventsStore.Abstractions
{
    public interface ICacheSettings
    {
        string GetValue(CacheVariable key);
    }
}
