using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using vietjet_series_booking_dotnet.App.database;
using vietjet_series_booking_dotnet.Modules.Intelisys.Entities;
using vietjet_series_booking_dotnet.App.Controllers;
using Newtonsoft.Json.Linq;
using System.Text;
using vietjet_series_booking_dotnet.Modules.Helper;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;

namespace vietjet_series_booking_dotnet.Modules.Intelisys.Controllers
{
    [Route("api/intelisys")]
    [ApiController]
    public class LoginController : BaseController
    {
        private readonly TokenController _token;
        private IConfiguration _config;
        public readonly MainContext _mainContext;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string _urlMain = "";

        public LoginController(IConfiguration config, MainContext mainContext)
        {
            _mainContext = mainContext;
            _config = config;
            _token = new TokenController(config, mainContext);
            _urlMain = config["UrlApi:Maint"];
        }
      
        //[Authorize]
        [HttpPost("login")]
        public IActionResult Login([FromBody] JObject jo)
        {
            try
            {
                JToken jt;
                if (jo.TryGetValue("username", out jt) && jo.TryGetValue("password", out jt))
                {
                    string user_name = jo["username"].ToString();
                    string user_pass = jo["password"].ToString();
                    //select from user_permissions
                    var user_prms = _mainContext.user_permissions.Where(x => x.username.Equals(user_name)).FirstOrDefault();
                    //Create Token
                    object requestToken = _token.CreateAccessToken(user_name, user_pass).Result;
                    var jtoken = JObject.FromObject(requestToken);
                    if (!jtoken["status_code"].ToString().Equals("200"))
                        return ResponseData(jtoken["data"].ToString(), jtoken["message"].ToString(), int.Parse(jtoken["status_code"].ToString()));
                    //Take token
                    var token = _mainContext.tokens.Where(x => x.access_token.Equals(jtoken["data"]["access_token"].ToString())).FirstOrDefault();
                    string bearer = $"Bearer { jtoken["data"]["token_user"].ToString()}";
                    var param = new Dictionary<string, object>()
                    {
                        { "Authorization",bearer }
                    };

                    //Agencies
                    (string agencies,int? statuscode) = HttpClientHelper.SendRequestAsync($"{_urlMain}agencies", HttpMethod.Get, param).Result;
                    var j_agencies = JArray.Parse(agencies.ToString());
                    var agencies_key = System.Net.WebUtility.UrlEncode(j_agencies[0]["key"].ToString());
                    //Users
                    (string users, int? statuscode2)  = HttpClientHelper.SendRequestAsync($"{_urlMain}agencies/{agencies_key}/users", HttpMethod.Get, param).Result;
                    var j_user = JArray.Parse(users.ToString());
                    var user_key = "";
                    for (int i = 0; i < j_user.Count(); i++)
                    {
                        if (j_user[i]["userLogonName"].ToString().ToLower() == user_name.ToLower())
                        {
                            user_key = j_user[i]["key"].ToString();
                        }
                    }
                    //Groups
                    (string groups, int? statuscode3)  = HttpClientHelper.SendRequestAsync($"{_urlMain}users/{user_key}/groups", HttpMethod.Get, param).Result;
                    var j_groups = JArray.Parse(groups.ToString());
                    string[] groups_name = new string[j_groups.Count()];
                    for (int i = 0; i < j_groups.Count(); i++)
                    {
                        groups_name[i] = j_groups[i]["group"]["name"].ToString();

                    }
                    bool flag = false;
                    foreach (var group_name in groups_name)
                    {
                        if (group_name.ToLower() == "cs" || group_name.ToLower() == "group")
                            flag = true;
                    }
                    if (flag == false && user_name.ToLower() != "vietjetaltaapi" && user_name.ToLower() != "vjco99")
                    {
                        _mainContext.Remove(token);
                        _mainContext.SaveChanges();
                        return ResponseForbidden($"User {user_name} error: Group user is not accept");
                    }
                    //Permission
                    (string permission_STR, int? statuscode4) permission_str = HttpClientHelper.SendRequestAsync($"{_urlMain}users/{user_key}/effectivePermissions", HttpMethod.Get, param).Result;
                    var j_permission = JArray.Parse(permission_str.permission_STR);
                    string[] prms = new string[j_permission.Count()];
                    string permission = "Not permission";
                    for (int i = 0; i < j_permission.Count(); i++)
                    {
                        if (bool.Parse(j_permission[i]["authorized"].ToString()))
                        {
                            prms[i] = j_permission[i]["name"].ToString();
                        }
                    }
                    //Decentralization
                    if (prms.Contains("Group Booking - Override") && prms.Contains("Group Booking - Access") && prms.Contains("Fare Setup - Access") && prms.Contains("Fare Setup - Modify"))
                        permission = "Full permission";
                    else if (prms.Contains("Group Booking - Override"))
                        permission = "Booking override permission";
                    else if (prms.Contains("Group Booking - Access"))
                        permission = "Booking access permission";
                    else if (prms.Contains("Fare Setup - Access"))
                        permission = "Fare access permission";
                    else if (prms.Contains("Fare Setup - Modify"))
                        permission = "Fare modify permission";
                    if (user_name.ToLower() == "vjco99" || user_name.ToLower() == "vietjetaltaapi")
                    {
                        permission = "SuperAdmin";
                    }
                    if (permission != "Not permission")
                    {
                        token.permission = permission;
                        _mainContext.SaveChanges();
                    }
                    else
                    {
                        _mainContext.Remove(token);
                        _mainContext.SaveChanges();
                        return ResponseUnauthorized($"User {user_name} error: User have not permission");
                    }
                    Log log = new Log(user_name, "Login", jtoken["data"]["access_token"].ToString());
                    _mainContext.logs.Add(log);
                    _mainContext.SaveChanges();
                    return ResponseOk(new { token = jtoken["data"], permission }, jtoken["message"].ToString());
                }
                else
                {
                    return ResponseBadRequest("Request invalid"); 

                }
            } 
            catch(Exception ex)
            {
                return ResponseBadRequest(ex.Message);
            }
            
        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                string header = Request.Headers["Authorization"];
                object check_token = null;
                check_token = _token.CheckToken(header.Substring(7)).Result;
                var j_check_token = JObject.FromObject(check_token);
                if (!j_check_token["data"].HasValues)
                {
                    return ResponseData(j_check_token["data"], j_check_token["message"].ToString(), int.Parse(j_check_token["status_code"].ToString()));
                }
                var username = _mainContext.tokens.Where(x => x.access_token.Equals(j_check_token["data"]["access_token"].ToString())).FirstOrDefault().username;
                Log log = new Log(username, "Logout", j_check_token["data"]["access_token"].ToString());
                _mainContext.logs.Add(log);
                _mainContext.SaveChanges();
                _token.DestroyToken(j_check_token["data"]["access_token"].ToString());
                return ResponseOk(null,"Logout Success");
            }
            catch(Exception ex)
            {
                return ResponseUnauthorized (ex.Message);
            }
        }
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            try
            {
                string header = Request.Headers["Authorization"];
                object refresh_token = null;
                refresh_token = _token.RefeshToken(header.Substring(7)).Result;
                var j_refresh_token = JObject.FromObject(refresh_token);
                return ResponseOk(j_refresh_token);
            }
            catch (Exception ex)
            {
                return ResponseUnauthorized(ex.Message);
            }
        }
    }
}
