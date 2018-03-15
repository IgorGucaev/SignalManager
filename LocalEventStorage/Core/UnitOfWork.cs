using EventsManager.LocalEventStorage.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.LocalEventStorage.Core
{
    public abstract class UnitOfWork<TDbContext> : IUnitOfWork
        where TDbContext : DbContext, new()
    {
        protected TDbContext _dbContext;

        public TDbContext Context
        {
            get
            {
                if (_dbContext == null)
                    _dbContext = new TDbContext();

                return _dbContext;
            }
        }

        public virtual void Dispose()
        {
            _dbContext?.Dispose();
        }

        public DbContext GetContext()
        {
            return this.Context;
        }
    }
}