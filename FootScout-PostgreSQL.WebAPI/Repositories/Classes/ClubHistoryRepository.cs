using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootScout_PostgreSQL.WebAPI.Repositories.Classes
{
    public class ClubHistoryRepository : IClubHistoryRepository
    {
        private readonly AppDbContext _dbContext;

        public ClubHistoryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ClubHistory> GetClubHistory(int clubHistoryId)
        {
            return await _dbContext.ClubHistories
                .Include(ch=> ch.PlayerPosition)
                .Include(ch => ch.Achievements)
                .Include(ch => ch.Player)
                .FirstOrDefaultAsync(ch => ch.Id == clubHistoryId);
        }

        public async Task<IEnumerable<ClubHistory>> GetAllClubHistory()
        {
            return await _dbContext.ClubHistories
                .Include(ch => ch.PlayerPosition)
                .Include(ch => ch.Achievements)
                .Include(ch => ch.Player)
                .ToListAsync();
        }

        public async Task<int> GetClubHistoryCount()
        {
            return await _dbContext.ClubHistories.CountAsync();
        }

        public async Task CreateClubHistory(ClubHistory clubHistory)
        {
            if (clubHistory.StartDate.Kind == DateTimeKind.Local)
                clubHistory.StartDate = clubHistory.StartDate.ToUniversalTime();
            else if (clubHistory.StartDate.Kind == DateTimeKind.Unspecified)
                clubHistory.StartDate = DateTime.SpecifyKind(clubHistory.StartDate, DateTimeKind.Local).ToUniversalTime();

            if (clubHistory.EndDate.Kind == DateTimeKind.Local)
                clubHistory.EndDate = clubHistory.EndDate.ToUniversalTime();
            else if (clubHistory.EndDate.Kind == DateTimeKind.Unspecified)
                clubHistory.EndDate = DateTime.SpecifyKind(clubHistory.EndDate, DateTimeKind.Local).ToUniversalTime();

            await _dbContext.ClubHistories.AddAsync(clubHistory);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateClubHistory(ClubHistory clubHistory)
        {
            if (clubHistory.StartDate.Kind == DateTimeKind.Local)
                clubHistory.StartDate = clubHistory.StartDate.ToUniversalTime();
            else if (clubHistory.StartDate.Kind == DateTimeKind.Unspecified)
                clubHistory.StartDate = DateTime.SpecifyKind(clubHistory.StartDate, DateTimeKind.Local).ToUniversalTime();

            if (clubHistory.EndDate.Kind == DateTimeKind.Local)
                clubHistory.EndDate = clubHistory.EndDate.ToUniversalTime();
            else if (clubHistory.EndDate.Kind == DateTimeKind.Unspecified)
                clubHistory.EndDate = DateTime.SpecifyKind(clubHistory.EndDate, DateTimeKind.Local).ToUniversalTime();

            _dbContext.ClubHistories.Update(clubHistory);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteClubHistory(int clubHistoryId)
        {
            var clubHistory = await _dbContext.ClubHistories.FindAsync(clubHistoryId);
            if (clubHistory == null)
                throw new ArgumentException($"No club history found with ID {clubHistoryId}");

            if (clubHistory.AchievementsId != null)
            {
                var achievements = await _dbContext.Achievements.FindAsync(clubHistory.AchievementsId);
                if (achievements != null)
                    _dbContext.Achievements.Remove(achievements);
            }

            _dbContext.ClubHistories.Remove(clubHistory);
            await _dbContext.SaveChangesAsync();
        }
    }
}