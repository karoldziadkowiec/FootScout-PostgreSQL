using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Interfaces;

namespace FootScout_PostgreSQL.WebAPI.Repositories.Classes
{
    public class AchievementsRepository : IAchievementsRepository
    {
        private readonly AppDbContext _dbContext;

        public AchievementsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateAchievements(Achievements achievements)
        {
            await _dbContext.Achievements.AddAsync(achievements);
            await _dbContext.SaveChangesAsync();
        }
    }
}