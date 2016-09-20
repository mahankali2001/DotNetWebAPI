using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using SampleWebAPI.Context;
using SampleWebAPI.Utility;

namespace SampleWebAPI.Repository
{
    public interface IEntityRepository
    {
    }

    public abstract class EntityRepository : IEntityRepository
    {
        protected EntityRepository(RepositoryContext context)
        {
            Context = context;
        }

        private RepositoryContext Context { get; set; }

        protected IDatabaseContext DatabaseContext
        {
            get { return Context.DatabaseContext; }
        }

        protected IQueryable<T> Read<T>() where T : class
        {
            return Context.Read<T>();
        }

        protected virtual void InsertEntity<T>(T entity) where T : class
        {
            NavigationBackupSet backupSet = null;
            try
            {
                backupSet = NavigationBackup<T>.BackupAndReset(entity);
                Context.Write<T>().Add(entity);
                Context.SaveChanges();
                Context.Entry(entity).State = EntityState.Detached;
            }
            finally
            {
                NavigationBackup<T>.Restore(entity, backupSet);
            }
        }

        protected virtual void UpdateEntity<T>(T entity) where T : class
        {
            NavigationBackupSet backupSet = null;
            try
            {
                backupSet = NavigationBackup<T>.BackupAndReset(entity);
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();
                Context.Entry(entity).State = EntityState.Detached;
            }
            finally
            {
                NavigationBackup<T>.Restore(entity, backupSet);
            }
        }

        protected virtual void DeleteEntity<T>(T entity) where T : class
        {
            NavigationBackupSet backupSet = null;
            try
            {
                backupSet = NavigationBackup<T>.BackupAndReset(entity);
                Context.Write<T>().Attach(entity);
                Context.Write<T>().Remove(entity);
                Context.SaveChanges();
            }
            finally
            {
                NavigationBackup<T>.Restore(entity, backupSet);
            }
        }

        protected virtual void SaveEntityWithAutoId<T>(T entity, int id) where T : class
        {
            if (!Helper.IsValidId(id))
            {
                InsertEntity(entity);
            }
            else
            {
                UpdateEntity(entity);
            }
        }

        protected virtual void SaveEntityWithAutoId<T>(T entity, long id) where T : class
        {
            if (!Helper.IsValidId(id))
            {
                InsertEntity(entity);
            }
            else
            {
                UpdateEntity(entity);
            }
        }

        protected virtual void Save<T>(T entity, Action<T> persistAction)
        {
            persistAction(entity);
        }
    }
}