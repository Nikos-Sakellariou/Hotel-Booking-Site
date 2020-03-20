using System;
using System.Collections.Generic;

namespace HotelBooking.Models
{
    public partial class SuperBooking
    { 
        public int BookingId { get; set; }
        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }
        public int RoomId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string Photo { get; set; }
        public string RoomType { get; set; }
        public int Price { get; set; }
        public string ShortDescription { get; set; }
    }
}
