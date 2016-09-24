using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SampleApp.Core.Data;
using SampleWebAPI.Entities;

namespace SampleWebAPI.DTO
{
    public class UserDTO
    {
        public int id { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public UserDTO(JQGUser u)
        {
            id = u.uid;
            firstName = u.firstName;
            lastName = u.lastName;
        }
    }
}