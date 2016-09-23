using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using SampleApp.Core.Data;

namespace SampleWebAPI.Entities
{
    [Table("JQGUser")]
    public class JQGUser
    {
        [Key]
        public int uid { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }
    }
}