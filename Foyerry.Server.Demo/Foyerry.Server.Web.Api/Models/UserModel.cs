using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foyerry.Server.Web.Api.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public double Income { get; set; }
        public DateTime BirthDate { get; set; }
    }
}