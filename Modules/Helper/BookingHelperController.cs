using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using vietjet_series_booking_dotnet.App.Controllers;
using vietjet_series_booking_dotnet.App.database;

namespace vietjet_series_booking_dotnet.Modules.Helper
{
    [Route("api/")]
    [ApiController]
    public class BookingHelperController : BaseController
    {
        private IConfiguration _config;
        public readonly MainContext _mainContext;
        private readonly string _urlMain = "";

        public BookingHelperController(IConfiguration config, MainContext mainContext)
        {
            _mainContext = mainContext;
            _config = config;
            _urlMain = config["UrlApi:Maint"];
        }
        public object getTravelOptions(string accesstoken,string citypair,DateTime departure,string currency,int adult,int child = 0,int infant = 0,string cabinclass = "Y")
        {
            try
            {
                var header = new Dictionary<string, object>
                {
                    {"Authorization",$"Bearer {accesstoken}"}
                };
                string url = GetQueryTravelOptions("travelOptions",citypair,departure,currency,adult,child,infant,cabinclass);
                (string content, int? statuscode) travelOptions = HttpClientHelper.SendRequestAsync(url,HttpMethod.Get,header).Result;
                var j_travelOptions = JArray.Parse(travelOptions.content);
                return new { data = j_travelOptions, status_code = travelOptions.statuscode };
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        public string GetQueryTravelOptions(string endpoint ,string citypair, DateTime departure, string currency, int adult, int child = 0, int infant = 0, string cabinclass = "Y")
        {
            return $"{_urlMain}{endpoint}?cityPair={citypair}&departure={departure.ToString("yyyy/MM/dd")}&cabinClass={cabinclass}&currency={currency}&adultCount={adult}&childCount={child}&infantCount={infant}";
        }
    }
}