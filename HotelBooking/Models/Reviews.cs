using System;
using System.Collections.Generic;

namespace HotelBooking.Models
{
    public partial class Reviews
    {
        public int ReviewId { get; set; }
        public int Rate { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
    }
}
