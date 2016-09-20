using System.Collections.ObjectModel;
using SampleWebAPI.Context;
using SampleWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace SampleWebAPI.Repository
{
    public interface IJQGUserRepository : IEntityRepository
    {
        string GetHello(string name);
        DataTable GetUsers();

        DataTable GetPagedUsers(int uid, int pageIndex, int pageSize, string filters, string sortColumn,
                           string sortOrder, int active);

        User GetUser(int uid);

        void Save(User user);
    }

    public class JQGUserRepository : EntityRepository, IJQGUserRepository
    {
        
        public JQGUserRepository(RepositoryContext context): base(context)
        {}

        public string GetHello(string name)
        {
            // Sample code for Database Access
            // string sampleData = String.Join("," , this.Read<TEMPLATEEntity>().ToList().Select(d => d.Data));
            return String.Format("Hello {0}", name);
        }

        public DataTable GetUsers()
        {
             return this.DatabaseContext.ExecuteReader("select * from [dbo].[user]", CommandType.Text , null);
        }

        public DataTable GetPagedUsers(int uid, int pageIndex, int pageSize, string filters, string sortColumn, string sortOrder, int active)
        {
            return this.DatabaseContext.ExecuteReader("GetUsers", CommandType.StoredProcedure,
                                                                  this.DatabaseContext.CreateParameter("@PageIndex", pageIndex),
                                                                  this.DatabaseContext.CreateParameter("@PageSize", pageSize),
                                                                  this.DatabaseContext.CreateParameter("@FilterData", filters),
                                                                  this.DatabaseContext.CreateParameter("@SortColumn", sortColumn),
                                                                  this.DatabaseContext.CreateParameter("@SortOrder", sortOrder),                                                  
                                                                  this.DatabaseContext.CreateParameter("@UserId", uid),
                                                                  this.DatabaseContext.CreateParameter("@Active", active)
                );
        }

        public User GetUser(int uid)
        {
            //return this.DatabaseContext.ExecuteReader("select * from [dbo].[user] where uid=" + uid, CommandType.Text, null);
            var users = from u in this.Read<User>()
                        where u.uid == uid
                        select u;
            return users.ToList().FirstOrDefault();
        }

        public void Save(User entity)
        {
            this.SaveEntityWithAutoId(entity, entity.uid);
        }
    }
}

