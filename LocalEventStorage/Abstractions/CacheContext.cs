using EventsManager.LocalEventStorage.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.LocalEventStorage.Abstractions
{
    public class CacheContext : DbContext
    {
        public DbSet<Signal> Signals { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=cache.db");

        }
    }
}
