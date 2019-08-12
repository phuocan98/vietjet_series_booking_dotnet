using System;

namespace vietjet_series_booking_dotnet.Modules.Booking.Controllers
{
    internal class tempList
    {
        public int id { get; internal set; }
        public string type { get; set; }
        public string name { get; internal set; }
        public string phone { get; internal set; }
        public string email { get; internal set; }
        public string segmen1 { get; internal set; }
        public string flightNo1 { get; internal set; }
        public string fightTime1 { get; internal set; }
        public string fareClass1 { get; internal set; }
        public string segment2 { get; internal set; }
        public string flightNo2 { get; internal set; }
        public string fightTime2 { get; internal set; }
        public string fareClass2 { get; internal set; }
        public int amount { get; internal set; }
        public string currency { get; internal set; }
        public DateTime time1 { get; set; }
        public DateTime time2 { get; set; }
    }
}