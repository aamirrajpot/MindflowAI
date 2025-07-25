﻿using System.ComponentModel.DataAnnotations;

namespace MindflowAI.Services.Dtos.AppUser
{
    public class RegisterWithOtpDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
