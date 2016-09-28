using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using SampleWebAPI.Entities;
using SampleWebAPI.Repository;
using SampleWebAPI.DTO;
using SampleApp.Core.Context;
using System.Data.Common;
using System.Data.SqlClient;
using SampleApp.Core.Configuration;
using System.Web.Mvc;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace SampleWebAPI.Controllers
{

    //public class JsonContent : HttpContent
    //{
    //    private readonly JToken _value;

    //    public JsonContent(JToken value)
    //    {
    //        _value = value;
    //        Headers.ContentType = new MediaTypeHeaderValue("application/json");
    //    }

    //    protected override Task SerializeToStreamAsync(Stream stream,
    //        TransportContext context)
    //    {
    //        var jw = new JsonTextWriter(new StreamWriter(stream))
    //        {
    //            Formatting = Formatting.Indented
    //        };
    //        _value.WriteTo(jw);
    //        jw.Flush();
    //        //return Task.FromResult<object>(null);
    //        return null;
    //    }

    //    protected override bool TryComputeLength(out long length)
    //    {
    //        length = -1;
    //        return false;
    //    }
    //}

    public class JQGUsersAPIController : ApiController
    {
        //
        // GET: /User/

        //private List<User> userList = new List<User>();
        //public ActionResult Index()
        //{
        //    return View();
        //}

        
        //User[] users = new User[]
        //{
        //new User { uid = 1, firstName= "AF", lastName= "AL"},
        //new User { uid = 2, firstName= "BF", lastName= "BL"},
        //new User { uid = 3, firstName= "CF", lastName= "CL"},
        //};
        //public IEnumerable<User> GetUsers()
        //{
        //    return users;
        //}

        //private UserRepository ur = new UserRepository();

        //UserRepository ur =  new UserRepository();
        //public IEnumerable<User> Get() 
        //public List<User> Get()
        //{
        //    //return users;

        //    //userList.Add(new User {uid = 1, firstName = "AF", lastName = "AL"});
        //    //userList.Add(new User {uid = 2, firstName = "BF", lastName = "BL"});
        //    //userList.Add(new User {uid = 3, firstName = "CF", lastName = "CL"});
        //    //return userList;

        //    return ur.GetAllUsers();
        //}
        //public User Get(string id)
        //{
        //    return ur.GetUser(Int32.Parse(id));
        //}
        //public User Post(User user)
        //{
        //    return ur.AddUser(user);
        //}
        //public User Put(User user)
        //{
        //    return ur.UpdateUser(user);
        //}
        //public void Delete(string id)
        //{
        //    ur.RemoveUser(Int32.Parse(id));
        //}

        //[System.Web.Http.ActionName("Copy")]
        //public void CopyUser(string id)
        //{
        //    ur.CopyUser(Int32.Parse(id));
        //}

        //[System.Web.Http.ActionName("Copy")]
        //public JsonResult JQGGet()
        //{
        //    //return users;

        //    //userList.Add(new User {uid = 1, firstName = "AF", lastName = "AL"});
        //    //userList.Add(new User {uid = 2, firstName = "BF", lastName = "BL"});
        //    //userList.Add(new User {uid = 3, firstName = "CF", lastName = "CL"});
        //    //return userList;

        //    //return ur.GetAllUsers();
        //    return null;
        //}

        //26/09/16
        private static DbConnection connection = new SqlConnection(ConfigurationManager.GetAppSettingsValue("ConnectionString"));
        private static RepositoryContext context = new RepositoryContext(connection);
        private JQGUserRepository jqgUR = new JQGUserRepository(context);
        [System.Web.Http.ActionName("Users")] // not needed if it same as method name
        public JsonResult GetUsersLists(string sidx, string sord, int page, int rows)  //Gets the user Lists.
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            DTOPage<UserDTO> ur = jqgUR.GetUsers(sidx, sord, page, rows);

            JQGridData<UserDTO> jsonData = new JQGridData<UserDTO>()
            {
                total = ur.PageCount,
                page = page,
                records = ur.RowCount,
                rows = ur.Results
            };

           return  new System.Web.Mvc.JsonResult()
            {
                Data = jsonData,
                JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet
            };

            //return new HttpResponseMessage()
            //{
            //    Content = new JsonContent(jsonData)
            //};
            //return jsonData; //Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //private System.Web.Mvc.JsonResult Json(object jsonData, System.Web.Mvc.JsonRequestBehavior jsonRequestBehavior)
        //{
        //    throw new NotImplementedException();
        //}

    }
}
