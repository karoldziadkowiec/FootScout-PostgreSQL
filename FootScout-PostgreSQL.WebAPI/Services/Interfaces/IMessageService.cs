﻿using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Models.DTOs;

namespace FootScout_PostgreSQL.WebAPI.Services.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetAllMessages();
        Task<int> GetAllMessagesCount();
        Task<IEnumerable<Message>> GetMessagesForChat(int chatId);
        Task<int> GetMessagesForChatCount(int chatId);
        Task<DateTime> GetLastMessageDateForChat(int chatId);
        Task<Message> SendMessage(MessageSendDTO dto);
        Task DeleteMessage(int messageId);
    }
}