using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using Microsoft.EntityFrameworkCore;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Repositories
{
    public class ClubHistoryRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly AppDbContext _dbContext;
        private readonly ClubHistoryRepository _clubHistoryRepository;

        public ClubHistoryRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _clubHistoryRepository = new ClubHistoryRepository(_dbContext);
        }

        [Fact]
        public async Task GetClubHistory_ReturnsCorrectClubHistory()
        {
            // Arrange
            var clubHistoryId = 1;

            // Act
            var result = await _clubHistoryRepository.GetClubHistory(clubHistoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(clubHistoryId, result.Id);
            Assert.Equal("FC Barcelona", result.ClubName);
            Assert.Equal("La Liga", result.League);
        }

        [Fact]
        public async Task GetAllClubHistory_ReturnsAllClubHistory()
        {
            // Arrange & Act
            var result = await _clubHistoryRepository.GetAllClubHistory();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetClubHistoryCount_ReturnsCorrectNumberOfClubHistory()
        {
            // Arrange & Act
            var result = await _clubHistoryRepository.GetClubHistoryCount();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task CreateClubHistory_AddsNewClubHistory()
        {
            // Arrange
            _dbContext.Achievements.Add(new Achievements { NumberOfMatches = 100, Goals = 50, Assists = 50, AdditionalAchievements = "LM x2" });
            await _dbContext.SaveChangesAsync();
            var achievements = await _dbContext.Achievements.FirstOrDefaultAsync(ch => ch.AdditionalAchievements == "LM x2");

            var newClubHistory = new ClubHistory
            {
                PlayerPositionId = 14,
                PlayerPosition = new PlayerPosition { Id = 14, PositionName = "Centre-Forward" },
                ClubName = "Inter Miami",
                League = "MLS",
                Region = "USA",
                StartDate = DateTime.UtcNow.AddDays(300),
                EndDate = DateTime.UtcNow.AddDays(450),
                AchievementsId = achievements.Id,
                PlayerId = "leomessi"
            };

            // Act
            await _clubHistoryRepository.CreateClubHistory(newClubHistory);

            // Assert
            var result = await _dbContext.ClubHistories.FirstOrDefaultAsync(ch => ch.ClubName == "Inter Miami");
            Assert.NotNull(result);
            Assert.Equal("Inter Miami", result.ClubName);
            Assert.Equal("MLS", result.League);

            _dbContext.ClubHistories.Remove(result);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateClubHistory_UpdatesExistingClubHistory()
        {
            // Arrange
            var clubHistoryId = 2;
            var existingClubHistory = await _dbContext.ClubHistories.FindAsync(clubHistoryId);
            existingClubHistory.ClubName = "Updated Club Name";
            existingClubHistory.League = "Updated League";

            // Act
            await _clubHistoryRepository.UpdateClubHistory(existingClubHistory);

            // Assert
            var result = await _dbContext.ClubHistories.FindAsync(clubHistoryId);
            Assert.NotNull(result);
            Assert.Equal("Updated Club Name", result.ClubName);
            Assert.Equal("Updated League", result.League);
        }

        [Fact]
        public async Task DeleteClubHistory_RemovesClubHistoryAndAchievements()
        {
            // Arrange
            _dbContext.Achievements.Add(new Achievements { NumberOfMatches = 100, Goals = 50, Assists = 50, AdditionalAchievements = "LM" });
            await _dbContext.SaveChangesAsync();
            _dbContext.ClubHistories.Add(new ClubHistory { PlayerPositionId = 15, PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" }, ClubName = "Borussia Dortmund", League = "Bundesliga", Region = "Germany", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(150), AchievementsId = 3, PlayerId = "leomessi" });
            await _dbContext.SaveChangesAsync();

            var clubHistory = await _dbContext.ClubHistories.FirstOrDefaultAsync(ch => ch.ClubName == "Borussia Dortmund");
            if (clubHistory == null)
                throw new Exception("Test club history not found");

            // Act
            await _clubHistoryRepository.DeleteClubHistory(clubHistory.Id);

            // Assert
            var result = await _dbContext.ClubHistories.FindAsync(clubHistory.Id);
            Assert.Null(result);
        }
    }
}