using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootScout_PostgreSQL.WebAPI.Repositories.Classes
{
    public class PlayerPositionRepository : IPlayerPositionRepository
    {
        private readonly AppDbContext _dbContext;

        public PlayerPositionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<PlayerPosition>> GetPlayerPositions()
            => await _dbContext.PlayerPositions.ToListAsync();

        public async Task<int> GetPlayerPositionCount()
            => await _dbContext.PlayerPositions.CountAsync();

        public async Task<string> GetPlayerPositionName(int positionId)
            => await _dbContext.PlayerPositions.Where(p => p.Id == positionId).Select(p => p.PositionName).FirstOrDefaultAsync();

        public async Task<bool> CheckPlayerPositionExists(string positionName)
        {
            return await _dbContext.PlayerPositions
                .AnyAsync(p => p.PositionName == positionName);
        }

        public async Task CreatePlayerPosition(PlayerPosition playerPosition)
        {
            var isExists = await _dbContext.PlayerPositions.AnyAsync(p => p.PositionName == playerPosition.PositionName);
            if (isExists == true)
                throw new ArgumentException($"Position {playerPosition.PositionName} already exists!");

            await _dbContext.PlayerPositions.AddAsync(playerPosition);
            await _dbContext.SaveChangesAsync();
        }
    }
}