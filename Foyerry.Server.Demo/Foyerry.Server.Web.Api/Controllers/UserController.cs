using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Providers.Entities;
using Foyerry.Server.Web.Api.Models;

namespace Foyerry.Server.Web.Api.Controllers
{
    public class UserController : ApiController
    {
        public List<UserModel> userList = new List<UserModel>();
        public UserController()
        {
            userList = new List<UserModel>()
            {
                new UserModel(){Id = 1,Age =25,Name = "Mr.wang",Income = 120.0,BirthDate = new DateTime(1995,12,25)},
                 new UserModel(){Id = 2,Age =45,Name = "Mrs.Li",Income = 2500.0,BirthDate = DateTime.Now},
                  new UserModel(){Id = 3,Age =65,Name = "Mr.Niu",Income = 8000.0,BirthDate = DateTime.Now.AddYears(-5)},

            };
        }


        [HttpGet]
        public List<UserModel> List()
        {
            return userList;
        }

        [HttpGet]
        public UserModel Detail(int id)
        {
            return userList.FirstOrDefault(it => it.Id == id);
        }
    }
}
