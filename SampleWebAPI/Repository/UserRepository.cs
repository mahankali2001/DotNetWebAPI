using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SampleApp.Core.Data;
using SampleWebAPI.Models;

namespace SampleWebAPI.Repository
{
    public class UserRepository
    {
        private List<User> userList = new List<User>();
        private Users users = new Users();

        public List<User> GetAllUsers()
        {
            //Code logic to get all students.
            //userList.Add(new User { uid = 1, firstName = "AF", lastName = "AL" });
            //userList.Add(new User { uid = 2, firstName = "BF", lastName = "BL" });
            //userList.Add(new User { uid = 3, firstName = "CF", lastName = "CL" });
            //return userList;

            return users.GetById(-1).ToList();
        }
        public User GetUser(int uid)
        {
            List<User> ul = users.GetById(uid).ToList();
            return (ul.Count==0)?new User() :ul.FirstOrDefault();
        }
        public void RemoveUser(int uid)
        {
            users.Delete(uid);
        }
        public User AddUser(User user)
        {
            user.uid = users.Insert(user);
            return user;
        }
        public User UpdateUser(User user)
        {
            users.Update(user);
            return user;
        }

        public void CopyUser(int uid)
        {
            User u = GetUser(uid);
            u.uid = 0;
            users.Insert(u);
        }
    }
}