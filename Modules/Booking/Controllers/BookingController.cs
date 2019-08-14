using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using vietjet_series_booking_dotnet.App.Controllers;
using vietjet_series_booking_dotnet.App.database;
using vietjet_series_booking_dotnet.Modules.Intelisys.Controllers;
using vietjet_series_booking_dotnet.Modules.Intelisys.Entities;
using vietjet_series_booking_dotnet.Modules.Booking.Entities;
using vietjet_series_booking_dotnet.Modules.Helper;
using vietjet_series_booking_dotnet.Modules.Request;

namespace vietjet_series_booking_dotnet.Modules.Booking.Controllers
{
    [Route("api/booking")]
    [ApiController]

    public class BookingController : BaseController
    {
        private IConfiguration _config;
        public readonly MainContext _mainContext;
        private readonly string _urlMain = "";
        private readonly TokenController _token;
        private readonly BookingHelperController _bookinghelper;

        public BookingController(IConfiguration config, MainContext mainContext)
        {
            _mainContext = mainContext;
            _config = config;
            _urlMain = config["UrlApi:Maint"];
            _token = new TokenController(config, mainContext);
            _bookinghelper = new BookingHelperController(config, mainContext);
        }
        //[HttpGet("get-traveloptions")]
        //public Object GetTravelOptions([FromForm] TravelOptionRequest request)
        //{
        //        string citypair = request.citypair.ToString();
        //        DateTime departure = Convert.ToDateTime(request.departure.ToString());
        //        string currency = request.currency.ToString();
        //        int adult = int.Parse(request.adult.ToString());
        //        string header = Request.Headers["Authorization"];
        //        string access_token = header.Substring(7);
        //        object check_token = _token.CheckToken(access_token).Result;
        //        var j_check_token = JObject.FromObject(check_token);
        //        if (!j_check_token["data"].HasValues)
        //        {
        //            return ResponseData(j_check_token["data"], j_check_token["message"].ToString(), int.Parse(j_check_token["status_code"].ToString()));
        //        }
        //        var token = _mainContext.tokens.Where(x => x.access_token.Equals(header.Substring(7).ToString())).FirstOrDefault();
        //        var result = JObject.FromObject(_bookinghelper.getTravelOptions(token.real_token, citypair, departure, currency, adult));
        //        return ResponseData(result["data"]);
        //}

        [HttpPost("booking-import")]
        public async Task<IActionResult> BookingImport([FromForm] TravelOptionRequest request)
        {
            string header = Request.Headers["Authorization"];
            string access_token = header.Substring(7);
            object check_token = _token.CheckToken(access_token).Result;
            var j_check_token = JObject.FromObject(check_token);
            if (!j_check_token["data"].HasValues)
            {
                return ResponseData(j_check_token["data"], j_check_token["message"].ToString(), int.Parse(j_check_token["status_code"].ToString()));
            }
            var token = _mainContext.tokens.Where(x => x.access_token.Equals(header.Substring(7).ToString())).FirstOrDefault();
            string file_Name = "";
            var path = "";
            string folder = "";
            if (Request.HasFormContentType)
            {
                if (Request.Form.Files.Count != 0)
                {
                    string path_db = Path.Combine("BookingImport",DateTime.Now.ToString("dd-MM-yyyy"));
                    IFormFile files = Request.Form.Files[0];
                    file_Name = $"{token.username}_{files.FileName}";
                    folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",path_db);
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path_db, file_Name);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await files.CopyToAsync(stream);
                        stream.Close();
                    }
                    Booking_import booking_import = new Booking_import(token.id,token.username,access_token,$"{path_db}/{file_Name}", file_Name);
                    Log log = new Log(token.username, "Import booking", access_token);
                    _mainContext.booking_imports.Add(booking_import);
                    _mainContext.logs.Add(log);
                    _mainContext.SaveChanges();
                }
                else
                {
                    return ResponseData(null, "file missing", 402);
                }
            }
            else return BadRequest( new { message= "Request is not type form data" });
            
            FileInfo file = new FileInfo(path);
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorkbook xlWorkBook;
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                    int totalRows = workSheet.Dimension.Rows;
                    int totalColumns = workSheet.Dimension.Columns;
                    if(totalRows > 100000)
                    {
                        return ResponseData(null,"File error, please check import file again",422);
                    }
                    for (int i = 2; i <= totalRows; i++)
                    {
                        bool flag = false;
                        double rs;
                        Regex preg = new Regex(@"^[a-zA-Z0-9]*$");
                        Regex phone = new Regex(@"^\+?\d{1,3}?[- .]?\(?(?:\d{2,3})\)?[- .]?\d\d\d[- .]?\d\d\d\d$");
                        Regex mail = new Regex(@"^[a-z][a-z0-9_\.]{5,32}@[a-z0-9]{2,}(\.[a-z0-9]{2,4}){1,2}$");
                        //check type digit
                        if (double.TryParse(workSheet.Cells[i, 1].Value.ToString(), out rs) && double.TryParse(workSheet.Cells[i, 20].Value.ToString(), out rs) && preg.IsMatch(workSheet.Cells[i, 2].Value.ToString()) && preg.IsMatch(workSheet.Cells[i, 3].Value.ToString()) && mail.IsMatch(workSheet.Cells[i, 5].Value.ToString()) && phone.IsMatch(workSheet.Cells[i, 4].Value.ToString()) && workSheet.Cells[i, 21].Value.ToString().Length == 3)  
                        {
                            flag = true;
                        }
                        
                        if (flag == true)
                        {
                            int id = int.Parse(workSheet.Cells[i, 1].Value.ToString());
                            string type = workSheet.Cells[i, 2].Value.ToString();
                            string name = workSheet.Cells[i, 3].Value.ToString();
                            string phonee = workSheet.Cells[i, 4].Value.ToString();
                            string email = workSheet.Cells[i, 5].Value.ToString();
                            string segmen1 = workSheet.Cells[i, 6].Value.ToString();
                            DateTime time1 = Convert.ToDateTime($"{workSheet.Cells[i, 9].Value.ToString()}-{workSheet.Cells[i, 8].Value.ToString()}-{workSheet.Cells[i, 7].Value.ToString()}");
                            string flightNo1 = workSheet.Cells[i, 10].Value.ToString();
                            string flightTime1 = workSheet.Cells[i, 11].Value.ToString();
                            string fareClass1 = workSheet.Cells[i, 12].Value.ToString();
                            string segment2 = workSheet.Cells[i, 13].Value.ToString();
                            DateTime time2 = Convert.ToDateTime($"{workSheet.Cells[i, 16].Value.ToString()}-{workSheet.Cells[i, 15].Value.ToString()}-{workSheet.Cells[i, 14].Value.ToString()}");
                            string flightNo2 = workSheet.Cells[i, 17].Value.ToString();
                            string flightTime2 = workSheet.Cells[i, 18].Value.ToString();
                            string fareClass2 = workSheet.Cells[i, 19].Value.ToString();
                            int amount = int.Parse(workSheet.Cells[i, 20].Value.ToString());
                            string currencyy = workSheet.Cells[i, 21].Value.ToString();
                            Booking_Tasks booking_tasks = new Booking_Tasks(token.id,type,name,phonee,email, flightNo1, fareClass1,amount,currencyy,token.access_token,time1,flightTime1,i);
                            _mainContext.booking_tasks.Add(booking_tasks);
                            // sau khi insert 1 row thi lam
                            string citypair = request.citypair.ToString();
                            DateTime departure = Convert.ToDateTime(request.departure.ToString());
                            string currency = request.currency.ToString();
                            int adult = int.Parse(request.adult.ToString());
                            var result = JObject.FromObject(_bookinghelper.getTravelOptions(token.real_token, citypair, departure, currency, adult));
                        }
                        else
                        {
                            file.Delete();
                            return ResponseBadRequest("error format");
                        }
                    }
                    _mainContext.SaveChanges();
                    return ResponseOk("done");
                }
            }
            catch (Exception e)
            {
                return ResponseBadRequest(e.Message);
            }
        }
    }
}
