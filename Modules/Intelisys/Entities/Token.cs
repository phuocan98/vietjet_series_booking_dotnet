using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vietjet_series_booking_dotnet.Modules.Intelisys.Entities
{
    [Table("tokens")]
    public class Token
    {
        [Key]
        public int id { get; set; }
        public string username { get; set; }
        public string access_token { get; set; }
        public string permission { get; set; }
        public int update_new_file { get; set; }
        public string real_token { get; set; }
        public string refresh_token { get; set; }
        public string last_activity { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

        public Token()
        {

        }
        public Token(string _user_name, string _accesstoken, int _update_newfile, string _realtoken, string _refreshtoken)
        {
            username = _user_name;
            access_token = _accesstoken;
            update_new_file = _update_newfile;
            real_token = _realtoken;
            refresh_token = _refreshtoken;
            created_at = DateTime.Now;
            updated_at = DateTime.Now;
        }
    }
    
}
