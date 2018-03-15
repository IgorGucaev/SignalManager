using EventsManager.LocalEventStorage.Abstractions;
using System;

namespace EventsManager.Abstractions
{
    public interface IEventManager : IDisposable
    {
        int IntervalMemToLocal { get; }

        int IntervalLocalToCloud { get; }

        void RegisterEvent(Signal e);

        void Start();

        void Stop();
    }
}
