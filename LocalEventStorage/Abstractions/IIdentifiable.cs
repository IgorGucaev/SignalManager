using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.LocalEventStorage.Abstractions
{
    public interface IIdentifiable<T>
    {
        T Id { get; }
    }
}
