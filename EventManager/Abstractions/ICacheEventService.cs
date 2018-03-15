using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.LocalEventStorage.Abstractions
{
    public interface ICacheEventService
    {
        void Push(string deviceId, string serializedEvent);
    }
}
