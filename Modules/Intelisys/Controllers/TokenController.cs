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
using System.Net;
using vietjet_series_booking_dotnet.Modules.Helper;
using System.Text;
using System.Net.Http;
using vietjet_series_booking_dotnet.Modules.Intelisys.Entities;
using Newtonsoft.Json;

namespace vietjet_series_booking_dotnet.Modules.Intelisys.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class TokenController : BaseController
    {
        private IConfiguration _config;
        public readonly MainContext _mainContext;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string url_main = "";

        public TokenController(IConfiguration config, MainContext mainContext)
        {
            _mainContext = mainContext;
            _config = config;
            url_main = config["UrlApi:Maint"];
        }
        public async Task<object> CreateAccessToken(string username,string password)
        {
            //Create Token
            var user_prms = _mainContext.user_permissions.Where(x => x.username.Equals(username)).FirstOrDefault();
            if (user_prms == null)
            {
                return new { status_code = 403, message = "Don't have permission", data = "" };
            }
            var response = await GetAccessToken(username, password);
            var jo = JObject.FromObject(response);
            if (jo["data"].ToString() == "")
            {
                return new { status_code = 401, message = jo["message"], data = jo["data"] };
            }
            return new { status_code = 200, message = jo["message"], data = jo["data"] };
        }
        public async Task<object> GetAccessToken(string username,string password)
        {
            var user_prms = _mainContext.user_permissions.Where(x => x.username.Equals(username)).FirstOrDefault();
            var userInfo = $"{username}:{password}";
            byte[] data = ASCIIEncoding.ASCII.GetBytes(userInfo);
            string base64Encoded = $"Basic {Convert.ToBase64String(data)}";
            var param = new Dictionary<string, object>()
            {
                { "Authorization",base64Encoded },
                { "X-amelia-DistributionChannel","A" }
            };
            try
            {
                (string response,int? statuscode) = await HttpClientHelper.SendRequestAsync($"{url_main}UserSessions", HttpMethod.Post, param);
                var result = JObject.Parse(response);
                Token token = new Token(username, result["accessToken"].ToString(), user_prms.update_new_file, result["accessToken"].ToString(), result["refreshToken"].ToString());
                _mainContext.tokens.Add(token);
                _mainContext.SaveChanges();
                return new { data = new { access_token = result["accessToken"],username = username, update_new_file=user_prms.update_new_file, token_user = result["accessToken"] }, message = "Login Success" };
            }       
           catch(Exception e)
            {
                return new { data = "", message = "Invalid credentials" };
            }
        }
        public async Task<object> CheckToken(string access_token)
        {
            var token = _mainContext.tokens.Where(x => x.access_token.Equals(access_token) || x.real_token.Equals(access_token)).FirstOrDefault();
            if ((token == null) || (access_token == null)) 
            {

                return new {check = false, data = token, message = "Check token error: Invalid Token", status_code = 401 };
            }
            if (token.updated_at.Subtract(DateTime.MinValue).TotalSeconds >= (DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds - 60 * 5))
            {
                return new { check = true, data = token, message = "", status_code = 200 };
            }
            string str = $"Bearer {token.real_token}";
            var param = new Dictionary<string, object>()
            {
                {"Authorization", str}
            };
            (string str, int? statuscode)request_check = await HttpClientHelper.SendRequestAsync($"{url_main}settings", HttpMethod.Get, param);
            if (request_check.statuscode == 200)
            {
                return new { check = true, data = token, message = "", status_code = 200 };
            }
            return new { check = false, data = "", message = "Check token error: Invalid Token", status_code = 401 };
        }

        public async Task<object> RefeshToken(string access_token)
        {
            var token = _mainContext.tokens.Where(x => x.access_token.Equals(access_token) || x.real_token.Equals(access_token)).FirstOrDefault();
            if ((token == null) || (access_token == null))
            {
                return new { data = new { username = token.username, access_token = token.access_token, permission = token.permission, update_new_file = token.update_new_file }, message = "Refresh token error: Invalid Token", status_code = 401 };
            }
            if (token.updated_at.Subtract(DateTime.MinValue).TotalSeconds >= (DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds - 60 * 5))
            {
                return new { data = new { username = token.username, access_token = token.access_token, permission = token.permission, update_new_file = token.update_new_file }, message = "Not refresh token", status_code = 200 };
            }
            var jsonObject = new
            {
                refreshToken = token.refresh_token
            };
            var json = JsonConvert.SerializeObject(jsonObject);
            (string str, int? status_code) requestrefresttoken = await HttpClientHelper.PutCallAPI($"{url_main}UserSessions", json);
            var respone = JObject.Parse(requestrefresttoken.str);
            if (requestrefresttoken.status_code != 200)
            {
                return ResponseData(null, "", (int)requestrefresttoken.status_code);
            }
            token.real_token = respone["accessToken"].ToString();
            token.refresh_token = respone["refreshToken"].ToString();
            _mainContext.SaveChanges();
            return new { data = new { username = token.username, access_token = token.access_token, permission = token.permission, update_new_file = token.update_new_file }, message = "Refresh token success", status_code = 200 };
        }

        public int DestroyToken(string access_token)
        {
            var token = _mainContext.tokens.Where(x => x.access_token.Equals(access_token)).FirstOrDefault();
            _mainContext.Remove(token);
            _mainContext.SaveChanges();
            return 1;
        }
    }
}
