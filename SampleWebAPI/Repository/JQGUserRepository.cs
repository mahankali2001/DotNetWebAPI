using System.Collections.ObjectModel;
using SampleApp.Core.Context;
using SampleApp.Core.Entities.Common;
using SampleApp.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using SampleWebAPI.DTO;
using SampleWebAPI.Entities;

namespace SampleWebAPI.Repository
{
    public interface IJQGUserRepository : IEntityRepository
    {
        string GetHello(string name);
        //DataTable GetUsers();
        DTOPage<UserDTO> GetUsers(string sidx, string sord, int page, int rows);

        string Create(UserDTO uDTO);

        DataTable GetPagedUsers(int uid, int pageIndex, int pageSize, string filters, string sortColumn,
                           string sortOrder, int active);

        User GetUser(int uid);

        void Save(User user);

        void Delete(int uid);

        string Update(UserDTO uDTO);
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

        public DTOPage<UserDTO> GetUsers(string sidx, string sord, int pageIndex, int pageSize)  //Gets the todo Lists.
        {
            var userListsResults = from u in this.Read<JQGUser>()
                                   orderby u.firstName
                                   select u;
            //int totalrecords = userListsResults.Count(); //100
            //int pageCount = totalrecords / pageSize; //10 //page=2
            //int skip = (pageIndex - 1) * pageSize;
            //IList<T> Results = userListsResults.Skip(skip).Take(pageSize).ToList();

            PagedResult<JQGUser> r1Result = SampleApp.Core.Utility.Pager<JQGUser>.GetResult(userListsResults, pageIndex, pageSize);

            return ConvertToDTOPage(r1Result);

            //return this.DatabaseContext.ExecuteReader("select * from [dbo].[user]", CommandType.Text , null);
        }

        //private PagedResult<UserResponse> ConvertToPageResultDTO(PagedResult<User> t)
        //{
        //    var r = new PagedResult<UserResponse> { Results = new List<UserResponse>() };
        //    if (t.Results != null)
        //    {
        //        foreach (User v in t.Results)
        //        {
        //            var vdto = new UserResponse(v);
        //            r.Results.Add(vdto);
        //        }
        //    }
        //    r.CurrentPage = t.CurrentPage;
        //    r.PageCount = t.PageCount;
        //    r.PageSize = t.PageSize;
        //    r.RowCount = t.RowCount;
        //    return r;
        //}

        public DTOPage<UserDTO> ConvertToDTOPage(PagedResult<JQGUser> entities)
        {
            var d = new DTOPage<UserDTO>();
            if ((entities != null) && (entities.Results != null))
            {
                d.Results = entities.Results.Select(e => new UserDTO(e)).ToList();
                d.PageCount = entities.PageCount;
                d.PageSize = entities.PageSize;
                d.RowCount = entities.RowCount;
                d.CurrentPage = entities.CurrentPage;
            }
            return d;
        }

        public string Create(UserDTO uDTO)
        {
            User u = ConvertToUser(uDTO);

            this.Save(u);

            return "Created Successfully";
        }

        public User ConvertToUser(UserDTO uDTO)
        {
            var d = new User();
            if (uDTO != null)
            {
                d.uid = uDTO.id;
                d.firstName = uDTO.firstName;
                d.lastName = uDTO.lastName;
            }
            return d;
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

        public void Delete(int uid)
        {
            User u = GetUser(uid);

            this.DeleteEntity(u);
        }

        public string Update(UserDTO uDTO)
        {
            User u = ConvertToUser(uDTO);
            
            this.Save(u);

            return "Saved Successfully";
        }
    }
}

