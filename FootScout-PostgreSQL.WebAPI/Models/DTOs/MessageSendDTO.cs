﻿namespace FootScout_PostgreSQL.WebAPI.Models.DTOs
{
    public class MessageSendDTO
    {
        public int ChatId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
    }
}