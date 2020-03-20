using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models
{
    public partial class RoomSuperSearch
    {
        [Required]
        public IEnumerable<RoomModel> RoomModels { get; set; }
        [Required]
        public RoomSearch Roomsearch { get; set; }

        [Required]
        public string CheckInDate { get; set; }

        [Required]
        public string CheckOutDate { get; set; }

        [Required]
        public string SelectedCity { get; set; }

        [Required]
        public string SelectedRoomType { get; set; }

        [Required]
        public int MinPrice { get; set; }

        [Required]
        public int MaxPrice { get; set; }
    }
}
