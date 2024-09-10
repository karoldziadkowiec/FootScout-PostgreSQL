using FootScout_PostgreSQL.WebAPI.Entities;

namespace FootScout_PostgreSQL.WebAPI.Repositories.Interfaces
{
    public interface IPlayerPositionRepository
    {
        Task<PlayerPosition> GetPlayerPosition(int positionId);
        Task<IEnumerable<PlayerPosition>> GetPlayerPositions();
        Task<int> GetPlayerPositionCount();
        Task<string> GetPlayerPositionName(int positionId);
        Task<bool> CheckPlayerPositionExists(string positionName);
        Task CreatePlayerPosition(PlayerPosition playerPosition);
    }
}