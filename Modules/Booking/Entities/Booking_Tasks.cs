using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace vietjet_series_booking_dotnet.Modules.Booking.Entities
{
    [Table("booking_tasks")]
    public class Booking_Tasks
    {
        [Key]
        public int id { get; set; }
        public int import_id { get; set; }
        public int ref_id { get; set; }
        public int row { get; set; }
        public string route { get; set; }
        public string base_currency { get; set; }
        public string group_type { get; set; }
        public string company { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string city_pair { get; set; }
        public string code { get; set; }
        public DateTime departure { get; set; }
        public string hour { get; set; }
        public string real_hour { get; set; }
        public string fare_class { get; set; }
        public string real_fare_class { get; set; }
        public string price { get; set; }
        public int amount { get; set; }
        public string note { get; set; }
        public string access_token { get; set; }
        public string booking_key { get; set; }
        public string booking_class { get; set; }
        public string res_number { get; set; }
        public string res_key { get; set; }
        public string file_passenger { get; set; }
        public int is_purchase { get; set; }
        public int parent { get; set; }
        public string flight_choose { get; set; }
        public int status { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

        public Booking_Tasks()
        {

        }
        public Booking_Tasks(int id_import,string grouptype,string companyname,string phonee,string mail,string codee,string fareclass,int amountt,string currency,string accesstoken,DateTime deppature,string hourr,int roww)
        {
            import_id = id_import;
            group_type = grouptype;
            company = companyname;
            phone = phonee;
            email = mail;
            code = codee;
            fare_class = fareclass;
            amount = amountt;
            base_currency = currency;
            access_token = accesstoken;
            created_at = DateTime.Now;
            updated_at = DateTime.Now;
            status = -1;
            departure = deppature;
            ref_id = 1;
            route = "";
            city_pair = "";
            hour = "";
            real_hour = "";
            real_fare_class = "";
            price = "";
            note = "";
            booking_key = "";
            booking_class = "";
            res_number = "";
            res_key = "";
            file_passenger = "";
            is_purchase = 1;
            parent = 1;
            flight_choose = "";
            hour =hourr;
            row = roww;
        }
    }
    
}
