using EventsManager.Abstractions;
using EventsManager.LocalEventStorage.Abstractions;
using System;

namespace EventsManager.Core
{
    public class CacheEventService : ICacheEventService
    {
        const int MAX_SIGNAL_SIZE = 1000;
        private IEventManager _EventManager;

        public void Push(string deviceId, string serializedEvent)
        {
            if (serializedEvent.Length > MAX_SIGNAL_SIZE)
                throw new ArgumentException($"Signal size must be less than {MAX_SIGNAL_SIZE}.");

            this._EventManager.RegisterEvent(Signal.New(deviceId: deviceId, data: serializedEvent, timestamp: DateTime.Now));
        }

        public CacheEventService(IEventManager manager)
        {
            _EventManager = manager;
        }
    }
}