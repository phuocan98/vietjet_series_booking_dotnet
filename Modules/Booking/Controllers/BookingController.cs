using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using vietjet_series_booking_dotnet.App.Controllers;
using vietjet_series_booking_dotnet.App.database;
using vietjet_series_booking_dotnet.Modules.Intelisys.Controllers;

namespace vietjet_series_booking_dotnet.Modules.Booking.Controllers
{
    [Route("api/booking")]
    [ApiController]

    public class BookingController : BaseController
    {
        private IConfiguration _config;
        public readonly MainContext _mainContext;
        private readonly string _urlMain = "";

        public BookingController(IConfiguration config, MainContext mainContext)
        {
            _mainContext = mainContext;
            _config = config;
            _urlMain = config["UrlApi:Maint"];
        }
        [HttpPost("booking-import")]
        public async Task<IActionResult> BookingImport([FromForm] IFormFile files)
        {
            string file_Name = "";
            var path = "";
            string folder = "";
            if (Request.Form.Files.Count != 0)
            {
                files = Request.Form.Files[0];
                file_Name = $"import{files.FileName}";
                folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file_Name);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await files.CopyToAsync(stream);
                }
            }

            FileInfo file = new FileInfo(path);
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorkbook xlWorkBook;
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                    int totalRows = workSheet.Dimension.Rows;

                    List<tempList> temp = new List<tempList>();
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
                            temp.Add(new tempList
                            {
                                id = int.Parse(workSheet.Cells[i, 1].Value.ToString()),
                                type = workSheet.Cells[i, 2].Value.ToString(),
                                name = workSheet.Cells[i, 3].Value.ToString(),
                                phone = workSheet.Cells[i, 4].Value.ToString(),
                                email = workSheet.Cells[i, 5].Value.ToString(),
                                segmen1 = workSheet.Cells[i, 6].Value.ToString(),
                                time1 = Convert.ToDateTime($"{workSheet.Cells[i, 9].Value.ToString()}-{workSheet.Cells[i, 8].Value.ToString()}-{workSheet.Cells[i, 7].Value.ToString()}"),
                                flightNo1 = workSheet.Cells[i, 10].Value.ToString(),
                                fightTime1 = workSheet.Cells[i, 11].Value.ToString(),
                                fareClass1 = workSheet.Cells[i, 12].Value.ToString(),
                                segment2 = workSheet.Cells[i, 13].Value.ToString(),
                                time2 = Convert.ToDateTime($"{workSheet.Cells[i, 16].Value.ToString()}-{workSheet.Cells[i, 15].Value.ToString()}-{workSheet.Cells[i, 14].Value.ToString()}"),
                                flightNo2 = workSheet.Cells[i, 17].Value.ToString(),
                                fightTime2 = workSheet.Cells[i, 18].Value.ToString(),
                                fareClass2 = workSheet.Cells[i, 19].Value.ToString(),
                                amount = int.Parse(workSheet.Cells[i, 20].Value.ToString()),
                                currency = workSheet.Cells[i, 21].Value.ToString(),
                            });
                        }
                        else
                        {
                            file.Delete();
                            return ResponseBadRequest("error format");
                        }
                    }
                    _mainContext.SaveChanges();
                    return ResponseOk(temp, "done");
                }
            }
            catch (Exception e)
            {
                return ResponseBadRequest(e.Message);
            }
        }
    }
}
