using System;
using System.Data.Entity;
//using System.Data.Objects;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Data.Common;
using SampleWebAPI.Entities;

namespace SampleApp.Core.Context
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbConnection connection): base(connection, false)
        {
            Database.SetInitializer<RepositoryContext>(null);
            Configuration.LazyLoadingEnabled = false;
            this.databaseContext = new DatabaseContext(this);
        }

        private IDatabaseContext databaseContext;
        public IDatabaseContext DatabaseContext
        {
            get 
            {
                return this.databaseContext; 
            }
        }


        #region Public methods
        public IQueryable<T> Read<T>() where T : class
        {
            return this.Set<T>().AsNoTracking();
        }

        public DbSet<T> Write<T>() where T : class
        {
            return this.Set<T>();
        }
        #endregion

        #region Protected methods
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.HasDefaultSchema("calendar");

            #region Register Entities

            //modelBuilder.Entity<TEMPLATEEntity>();
            modelBuilder.Entity<User>();
            modelBuilder.Entity<JQGUser>();
            //modelBuilder.HasDefaultSchema("dbo").Entity<DBEntities.EmailAttributes>();
            //modelBuilder.Entity<DBEntities.EmailAttributes>();
            #endregion
        }
        #endregion
    }
}