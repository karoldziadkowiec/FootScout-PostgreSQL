﻿namespace FootScout_PostgreSQL.WebAPI.Models.DTOs
{
    public class ProblemCreateDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string RequesterId { get; set; }
    }
}