﻿using FootScout_PostgreSQL.WebAPI.Entities;

namespace FootScout_PostgreSQL.WebAPI.Repositories.Interfaces
{
    public interface IPlayerFootRepository
    {
        Task<IEnumerable<PlayerFoot>> GetPlayerFeet();
        Task<string> GetPlayerFootName(int footId);
    }
}