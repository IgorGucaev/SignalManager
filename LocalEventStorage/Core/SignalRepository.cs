using EventsManager.LocalEventStorage.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.LocalEventStorage.Core
{
    public class SignalRepository : BaseRepository<CacheContext, Signal>
    {
        public SignalRepository(CacheContext dbContext) : base(dbContext)
        {
        }
    }
}
