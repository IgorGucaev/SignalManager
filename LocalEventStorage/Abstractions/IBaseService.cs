using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.LocalEventStorage.Abstractions
{
    public interface IBaseService<TUnitOfwork>
        where TUnitOfwork : IUnitOfWork
    {
        TUnitOfwork UnitOfWork { get; }
    }
}
