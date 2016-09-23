using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using SampleWebAPI.Repository;

namespace SampleWebAPI.Controllers
{
    public class UsersController : Controller
    {
        //
        // GET: /User/

        //private List<User> userList = new List<User>();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Manage()
        {
            return View();
        }

        
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

        ////UserRepository ur =  new UserRepository();
        ////public IEnumerable<User> Get() 
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
        //    return ur.GetUser(id);
        //}
        //public void Post(User user)
        //{
        //    ur.AddUser(user);
        //}
        //public void Put(User user)
        //{
        //    ur.UpdateUser(user);
        //}
        //public void Delete(string id)
        //{
        //    ur.RemoveUser(id);
        //}

    }
}
