using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models
{
    public class BookingModel
    {
        [Required]
        public DateTime CheckInDate { get; set; }
        [Required]
        public DateTime CheckOutDate { get; set; }
        [Required]
        public Room Room { get; set; }
        public IEnumerable<ReviewModel> ReviewModel { get; set; }
        public string RoomDescr { get; set; }
    }
}
