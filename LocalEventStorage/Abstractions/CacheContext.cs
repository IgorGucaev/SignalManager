using EventsManager.LocalEventStorage.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EventsManager.LocalEventStorage.Abstractions
{
    public class CacheContext : DbContext
    {
        private readonly CacheSettings _Settings;
        public DbSet<Signal> Signals { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbFilepath"></param>
        public CacheContext(CacheSettings settings)
        {
            _Settings = settings;

            if (_Settings == null)
                throw new ArgumentException("Cache settings must be specified");

            if (!File.Exists(_Settings.DbFilepath))
                RestoreDatabase();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_Settings.DbFilepath}");
        }

        /// <summary>
        /// Restoring database if database file does not exists on disk
        /// </summary>
        private void RestoreDatabase()
        {
            if (!String.IsNullOrWhiteSpace(_Settings?.CreateDbScriptPath))
            {
                string sqlQuery = File.ReadAllText(_Settings.CreateDbScriptPath);
                base.Database.ExecuteSqlCommand(sqlQuery);
            } 
        }
    }
}