using EventsManager.LocalEventStorage.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventsManager.LocalEventStorage.Core
{
    public class BaseRepository<TDbContext, TEntity> : ICacheRepository<TEntity>
        where TDbContext : DbContext, new()
        where TEntity : class
    {
        public bool IsAutoSave { get; protected set; }
        public TDbContext DbContext { get; protected set; }
        public DbSet<TEntity> DbSet { get; protected set; }

        public virtual IQueryable<TEntity> QueryAll
        { get { return this.DbSet.AsNoTracking(); } }

        public BaseRepository(TDbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("dbContext");

            this.IsAutoSave = false;

            this.DbContext = dbContext;
            this.DbSet = this.DbContext.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> Add(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public TEntity Add(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Entity", "Can't add null Entity to DbContext");

            this.DbSet.Add(entity);

            if (this.IsAutoSave)
                this.SaveChanges();

            return entity;
        }

        public virtual bool Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Entity", "Can't delete null Entity to DbContext");

            this.DbSet.Attach(entity);
            this.DbSet.Remove(entity);

            if (this.IsAutoSave)
                this.SaveChanges();

            return true;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return this.QueryAll.ToList();
        }

        protected virtual int SaveChanges()
        {
            return DbContext.SaveChanges();
        }

        public bool Truncate()
        {
            var model = this.DbContext.Model;

            // Get all the entity types information contained in the DbContext class, ...
            var entityTypes = model.GetEntityTypes();

            // ... and get one by entity type information of "FooBars" DbSet property.
            var entityType = entityTypes.First(t => t.ClrType == typeof(TEntity));

            // The entity type information has the actual table name as an annotation!
            var tableNameAnnotation = entityType.GetAnnotation("Relational:TableName");
            var tableNameOfEntitySet = tableNameAnnotation.Value.ToString();

            this.DbContext.Database.ExecuteSqlCommand($"delete from {tableNameOfEntitySet};");
            //  this.DbSet.Remove(entity);

            return true;
        }
    }
}