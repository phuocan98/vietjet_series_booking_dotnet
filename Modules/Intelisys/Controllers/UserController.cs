using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using vietjet_series_booking_dotnet.App.Controllers;
using vietjet_series_booking_dotnet.App.database;
using vietjet_series_booking_dotnet.Modules.Intelisys.Entities;

namespace vietjet_series_booking_dotnet.Modules.Intelisys.Controllers
{
    [Route("api/intelisys")]
    [ApiController]
    public class UserController : BaseController
    {
        private IConfiguration _config;
        public readonly MainContext _mainContext;
        private readonly string url_main = "";

        public UserController(IConfiguration config, MainContext mainContext)
        {
            _mainContext = mainContext;
            _config = config;
            url_main = config["UrlApi:Maint"];
        }

        [HttpGet("getlistuser")]
        public IActionResult GetListUser()
        {
            var user_permissions = _mainContext.user_permissions.Select(u => u);
            return ResponseData(user_permissions);
        }

        [HttpPost("adduser")]
        public IActionResult AddUser([FromBody] JObject jo)
        {
            var username = jo["user_name"].ToString();
            if(string.IsNullOrEmpty(username))
            {
                return ResponseData(null, "Username is not empty",422);
            }
            if (_mainContext.user_permissions.Where(u=>u.username.Equals(username)).FirstOrDefault() != null)
            {
                return ResponseData(null, "User is exists in system", 422);
            }
            User_Permissions user_prms = new User_Permissions(username, 3, 1, 0);
            _mainContext.user_permissions.Add(user_prms);
            _mainContext.SaveChanges();
            return ResponseData(null, "Add user success");
        }

         
    }
}