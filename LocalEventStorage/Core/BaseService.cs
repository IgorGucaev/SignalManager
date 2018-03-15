using EventsManager.LocalEventStorage.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.LocalEventStorage.Core
{
    public abstract class BaseService<TUnitOfWork> : IBaseService<TUnitOfWork>
        where TUnitOfWork : IUnitOfWork
    {
        protected TUnitOfWork _uow;

        public TUnitOfWork UnitOfWork { get { return _uow; } }

        public BaseService(TUnitOfWork uow)
        {
            _uow = uow;
        }
    }
}
