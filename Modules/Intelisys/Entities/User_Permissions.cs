using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace vietjet_series_booking_dotnet.Modules.Itelisys.Entities
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

        //public User_Permissions(string user_name)
        //{
        //    username = user_name;
        //    update_new_file = 1;
        //    permission = 0;
        //    state = 1;
        //    state = 1;
        //    created_at = DateTime.Now;
        //    updated_at = DateTime.Now;
        //}
    }
    
}
