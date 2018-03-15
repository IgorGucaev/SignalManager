using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.LocalEventStorage.Abstractions
{
    public interface IRepository
    { }

    public interface ICacheRepository<T> : IRepository
        where T : class
    {
        IEnumerable<T> GetAll();

        void Add(IEnumerable<T> entities);

        T Add(T entity);

        bool Truncate();

        bool Delete(T entity);
        bool Delete(IEnumerable<T> entities);

        int Count();
    }
}