using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace vietjet_series_booking_dotnet.Modules.Request
{
    public class TravelOptionRequest
    {

        [Required]
        [MaxLength(255)]
        public string citypair { get; set; }

        [Required]
        public DateTime departure { get; set; }

        [Required]
        [MaxLength(255)]
        public string currency { get; set; }

        [Required]
        public int adult { get; set; }
    }
}
