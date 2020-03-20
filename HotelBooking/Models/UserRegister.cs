using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models
{
    public partial class UserRegister
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password",
            ErrorMessage="Password and Confirmation Password do not match")]
        public string ConfirmPassword { get; set; }
    }
}
