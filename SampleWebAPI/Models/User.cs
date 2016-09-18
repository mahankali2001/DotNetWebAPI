using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SampleApp.Core.Data;

namespace SampleWebAPI.Models
{
    public class User : AbstractModel
    {
        public int uid { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }
    }

    interface IUsers
    {
        int Insert(User user);

        int Update(User user);

        int Delete(int uid);

        IList<User> GetById(int uid);
    }

    public class Users : IUsers
    {
        public static User CreateUserModelInstance(DatabaseRow databaseRow)
        {
            User model = new User
            {
                uid = databaseRow.GetColumnValue<int>("uid"),
                firstName = databaseRow.GetColumnValue<string>("firstName"),
                lastName = databaseRow.GetColumnValue<string>("lastName"),
            };

            return model;
        }

        #region Implementation of IUsers

        public int Insert(User user)
        {
            SqlParameterList sqlParameterList = new SqlParameterList();

            sqlParameterList.Add<string>("@firstName", user.firstName);
            sqlParameterList.Add<string>("@lastName", user.lastName);
            //sqlParameterList.Add<DateTime>("@CreateDatetime", user.CreateDateTime);
            //sqlParameterList.Add<int>("@uid", user.uid);
            //sqlParameterList.Add<bool>("@IsActive", user.IsActive);

            AdoNetImpl.Instance.ExecuteInsertStoredProcedure("User_Insert", sqlParameterList, user);
            return user.Id;
        }

        public int Update(User user)
        {
            SqlParameterList sqlParameterList = new SqlParameterList();

            sqlParameterList.Add<int>("@uid", user.uid);
            sqlParameterList.Add<string>("@firstName", user.firstName);
            sqlParameterList.Add<string>("@lastName", user.lastName);

            return AdoNetImpl.Instance.ExecuteStoredProcedure("User_Update", sqlParameterList);
        }

        public int Delete(int uid)
        {
            SqlParameterList sqlParameterList = new SqlParameterList();

            sqlParameterList.Add<int>("@uid", uid);

            return AdoNetImpl.Instance.ExecuteStoredProcedure("User_Delete", sqlParameterList);
        }

        public IList<User> GetById(int uid)
        {
            SqlParameterList sqlParameterList = new SqlParameterList();

            sqlParameterList.Add<int>("@uid", uid);

            return AdoNetImpl.Instance.ExecuteStoredProcedure<User>("User_Select", sqlParameterList, CreateUserModelInstance);
        }

        #endregion
    }
}