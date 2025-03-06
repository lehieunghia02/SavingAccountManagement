﻿using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models.AccountModels.ResetPasswordModel
{
    public class ResetPasswordModel
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; } = string.Empty;


        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
