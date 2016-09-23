using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SampleApp.Core.Data;
using SampleWebAPI.Entities;

namespace SampleWebAPI.DTO
{
    public class UserResponse
    {
        public int uid { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public UserResponse(JQGUser u)
        {
            uid = u.uid;
            firstName = u.firstName;
            lastName = u.lastName;
        }
    }
}