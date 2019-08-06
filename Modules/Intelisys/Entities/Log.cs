using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace vietjet_series_booking_dotnet.Modules.Intelisys.Entities
{
    [Table("logs")]
    public class Log
    {
        [Key]
        public int id { get; set; }
        public string username { get; set; }
        public string action { get; set; }
        public int id_action { get; set; } 
        public string access_token { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public Log()
        {

        }
        public Log (string user_name,string Action,string accesstoken)
        {
            username = user_name;
            action = Action;
            access_token = accesstoken;
            created_at = DateTime.Now;
            updated_at = DateTime.Now;
        }
    }
}
