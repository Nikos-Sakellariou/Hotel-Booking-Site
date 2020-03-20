using System;
using System.Collections.Generic;

namespace HotelBooking.Models
{
    public partial class SuperReview
    {
        public Reviews Review { get; set; }

        public Room Room { get; set; }

    }
}
