using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace vietjet_series_booking_dotnet.Modules.Booking.Entities
{
    [Table("booking_imports")]
    public class Booking_import
    {
        [Key]
        public int id { get; set; }
        public int token_id {get;set;}
        public string token_username { get; set; }
        public string file_path { get; set; }
        public string file_name { get; set; }
        public string access_token { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int status { get; set; }
        public Booking_import()
        {

        }
        public Booking_import(int id_token,string username_token,string token_access,string path,string name) 
        {
            token_id = id_token;
            token_username = username_token;
            access_token = token_access;
            file_path = path;
            file_name = name;
            created_at = DateTime.Now;
            updated_at = DateTime.Now;
        }

    }
}
