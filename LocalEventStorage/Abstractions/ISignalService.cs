using System;
using System.Collections.Generic;

namespace EventsManager.LocalEventStorage.Abstractions
{
    public interface ISignalService : IDisposable
    {
        void Add(IEnumerable<Signal> signals);

        IEnumerable<Signal> GetAll();

        void Truncate();

        int Count();
    }
}
