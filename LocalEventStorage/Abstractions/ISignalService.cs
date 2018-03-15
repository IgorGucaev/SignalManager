using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.LocalEventStorage.Abstractions
{
    public interface ISignalService
    {
        void Add(IEnumerable<Signal> signals);

        IEnumerable<Signal> GetAll();

        void Truncate();

        int Count();
    }
}
