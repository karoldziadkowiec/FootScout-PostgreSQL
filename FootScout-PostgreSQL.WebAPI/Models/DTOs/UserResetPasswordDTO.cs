﻿namespace FootScout_PostgreSQL.WebAPI.Models.DTOs
{
    public class UserResetPasswordDTO
    {
        public string PasswordHash { get; set; }
        public string ConfirmPasswordHash { get; set; }
    }
}