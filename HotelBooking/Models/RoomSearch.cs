using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models
{
    public partial class RoomSearch
    {
        [Required]
        public IEnumerable<string> Cities { get; set; }

        [Required]
        public IEnumerable<string> RoomTypes { get; set; }
    }
}
