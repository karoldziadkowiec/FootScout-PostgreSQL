﻿using FootScout_PostgreSQL.WebAPI.Entities;

namespace FootScout_PostgreSQL.WebAPI.Services.Interfaces
{
    public interface IChatService
    {
        Task<Chat> GetChatById(int chatId);
        Task<IEnumerable<Chat>> GetChats();
        Task<int> GetChatCount();
        Task<int> GetChatIdBetweenUsers(string user1Id, string user2Id);
        Task CreateChat(Chat chat);
        Task DeleteChat(int chatId);
        Task<MemoryStream> ExportChatsToCsv();
    }
}