using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.LocalEventStorage.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext GetContext();
        
        // T GetRepository<T>() where T : IRepository;
    }
}
