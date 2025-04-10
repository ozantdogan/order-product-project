﻿using System.ComponentModel.DataAnnotations;

namespace OTD.Core.Models.Requests
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(320)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(320)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
    }

    public class ConfirmEmailRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(320)]
        public string Email { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string ConfirmationCode { get; set; }
    }

    public class ResendConfirmationCodeRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(320)]
        public string Email { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }
}
