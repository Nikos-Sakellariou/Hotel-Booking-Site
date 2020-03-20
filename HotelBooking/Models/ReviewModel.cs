using System;
using System.Collections.Generic;

namespace HotelBooking.Models
{
    public partial class ReviewModel
    {
        public string Username { get; set; }
        public int Rate { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
