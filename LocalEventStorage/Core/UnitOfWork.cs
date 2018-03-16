using EventsManager.LocalEventStorage.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace EventsManager.LocalEventStorage.Core
{
    public abstract class UnitOfWork<TDbContext> : IUnitOfWork
        where TDbContext : DbContext
    {
        protected TDbContext _dbContext;

        public TDbContext Context
        {
            get
            {
                return _dbContext;
            }
        }

        public UnitOfWork(TDbContext context)
        {
            _dbContext = context;
        }

        public virtual void Dispose()
        {
            _dbContext?.Dispose();
        }

        public DbContext GetContext()
        {
            return Context;
        }
    }
}