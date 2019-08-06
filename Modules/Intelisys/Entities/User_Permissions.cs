using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace vietjet_series_booking_dotnet.Modules.Intelisys.Entities
{
    [Table("user_permissions")]
    public class User_Permissions
    {
        [Key]
        public int id { get; set; }
        public string username { get; set; }
        public int update_new_file { get; set; }
        public int permission { get; set; }
        public int state { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

        public User_Permissions()
        {
        }

        public User_Permissions(string name,int prms , int statee , int update_new_f)
        {
            username = name;
            update_new_file = update_new_f;
            permission = prms;
            state = statee;
            created_at = DateTime.Now;
            updated_at = DateTime.Now;
        }

    }
    
}
