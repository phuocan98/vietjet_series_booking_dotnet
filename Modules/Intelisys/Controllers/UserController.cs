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

        [HttpGet("get-list-user")]
        public IActionResult GetListUser()
        {
            var user_permissions = _mainContext.user_permissions.Select(u => u);
            return ResponseData(user_permissions);
        }

        [HttpPost("add-user")]
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

        [HttpPost("delete-user")]
        public IActionResult DeleteUser([FromBody] JObject jo)
        {
            var username = jo["user_name"].ToString();
            var user = _mainContext.user_permissions.Where(u => u.username.Equals(username)).FirstOrDefault();
            if (string.IsNullOrEmpty(username))
            {
                return ResponseData(null, "Username is not empty");
            }
            if (user == null) 
            {
                return ResponseData(null, "User is not exists in system", 422);
            }
            _mainContext.Remove(user);
            _mainContext.SaveChanges();
            return ResponseData(null, "Delete user success");
        }
        [HttpPost("update-permission-user")]
        public IActionResult UpdatePermissionAsync([FromBody] JArray datas)
        {
                string[] user_name = new string[datas.Count()];
                int i = 0;
                foreach (var data in datas)
                {
                    user_name[i] = data["username"].ToString();
                    i++;
                }
                var users = _mainContext.user_permissions.Select(u => u);
                foreach (var user in users)
                {
                    if (user_name.Contains(user.username))
                    {
                        user.update_new_file = 1;
                    }
                    else
                    {
                        user.update_new_file = 0;
                    }
                }
                _mainContext.SaveChanges();
                return ResponseData(null, "Update user success");
        }
    }
}