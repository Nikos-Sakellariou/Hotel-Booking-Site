using System;
using System.Collections.Generic;

namespace HotelBooking.Models
{
    public partial class SuperFavorite
    {
        public Favorites Favorite { get; set; }

        public Room Room { get; set; }

    }
}
