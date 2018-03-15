using EventsManager.LocalEventStorage.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.LocalEventStorage.Core
{
    public class BaseUnitOfWork : UnitOfWork<CacheContext>
    {
    }
}
