using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models
{
    public partial class UserProfile
    {
        [Required]
        public IEnumerable<SuperBooking> sbookings { get; set; }

        [Required]
        public IEnumerable<SuperReview> sreviews { get; set; }

        [Required]
        public IEnumerable<SuperFavorite> sfavorites { get; set; }
    }
}
