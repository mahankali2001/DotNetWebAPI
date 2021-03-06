﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using SampleApp.Core.Configuration;
using SampleApp.Core.Context;
using SampleWebAPI.DTO;
using SampleWebAPI.Entities;
using SampleWebAPI.Repository;

namespace SampleWebAPI.Controllers
{
    public class JQGUsersController : Controller
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

        private static DbConnection connection = new SqlConnection(ConfigurationManager.GetAppSettingsValue("ConnectionString"));
        private static RepositoryContext context = new RepositoryContext(connection);
        private JQGUserRepository jqgUR = new JQGUserRepository(context);
        public JsonResult GetUsersLists(string sidx, string sord, int page, int rows)  //Gets the user Lists.
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            DTOPage<UserDTO> ur = jqgUR.GetUsers(sidx, sord, page, rows);
            
            var jsonData = new
            {
                total = ur.PageCount,
                page,
                records = ur.RowCount,
                rows = ur.Results
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // TODO:insert a new row to the grid logic here
        //[System.Web.Mvc.HttpPost]
        //public string Create([Bind(Exclude = "Id")] UserDTO objTodo)
        public string Create(UserDTO objTodo)
        {
            
            string msg;
            try
            {
                if (ModelState.IsValid)
                {
                    //db.TodoLists.Add(objTodo);
                    //db.SaveChanges();
                    msg = jqgUR.Create(objTodo);
                }
                else
                {
                    msg = "Validation data not successfull";
                }
            }
            catch (Exception ex)
            {
                msg = "Error occured:" + ex.Message;
            }
            return msg;
        }
        public string Edit(UserDTO objTodo)
        {
            string msg;
            try
            {
                if (ModelState.IsValid)
                {
                    //db.Entry(objTodo).State = EntityState.Modified;
                    //db.SaveChanges();
                    //msg = "Saved Successfully";

                    msg = jqgUR.Update(objTodo);
                }
                else
                {
                    msg = "Validation data not successfull";
                }
            }
            catch (Exception ex)
            {
                msg = "Error occured:" + ex.Message;
            }
            return msg;
        }
        public string Delete(int uid)
        {
            //TodoList todolist = db.TodoLists.Find(uid);
            //db.TodoLists.Remove(todolist);
            //db.SaveChanges();

            jqgUR.Delete(uid);

            return "Deleted successfully";
        }
    }
}
