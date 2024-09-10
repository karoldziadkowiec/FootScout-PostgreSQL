using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using FootScout_PostgreSQL.WebAPI.UnitTests.TestManager;

namespace FootScout_PostgreSQL.WebAPI.UnitTests.Repositories
{
    public class ClubHistoryRepositoryTests : TestBase
    {
        [Fact]
        public async Task GetClubHistory_ReturnsCorrectClubHistory()
        {
            // Arrange
            var options = GetDbContextOptions("GetClubHistory_ReturnsCorrectClubHistory");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedClubHistoryTestBase(dbContext);
                var _clubHistoryRepository = new ClubHistoryRepository(dbContext);

                // Act
                var result = await _clubHistoryRepository.GetClubHistory(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal("FC Barcelona", result.ClubName);
                Assert.Equal("La Liga", result.League);
            }
        }

        [Fact]
        public async Task GetAllClubHistory_ReturnsAllClubHistory()
        {
            // Arrange
            var options = GetDbContextOptions("GetAllClubHistory_ReturnsAllClubHistory");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedClubHistoryTestBase(dbContext);
                var _clubHistoryRepository = new ClubHistoryRepository(dbContext);

                // Act
                var result = await _clubHistoryRepository.GetAllClubHistory();

                // Assert
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async Task GetClubHistoryCount_ReturnsCorrectNumberOfClubHistory()
        {
            // Arrange
            var options = GetDbContextOptions("GetClubHistoryCount_ReturnsCorrectNumberOfClubHistory");

            using (var dbContext = new AppDbContext(options))
            {
                await SeedClubHistoryTestBase(dbContext);
                var _clubHistoryRepository = new ClubHistoryRepository(dbContext);

                // Act
                var result = await _clubHistoryRepository.GetClubHistoryCount();

                // Assert
                Assert.Equal(2, result);
            }
        }

        [Fact]
        public async Task CreateClubHistory_AddsNewClubHistory()
        {
            // Arrange
            var options = GetDbContextOptions("CreateClubHistory_AddsNewClubHistory");
            var newClubHistory = new ClubHistory
            {
                Id = 3,
                PlayerPositionId = 14,
                ClubName = "Inter Miami",
                League = "MLS",
                Region = "USA",
                StartDate = DateTime.Now.AddDays(300),
                EndDate = DateTime.Now.AddDays(450),
                AchievementsId = 3,
                PlayerId = "leomessi"
            };

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedClubHistoryTestBase(dbContext);
                var _clubHistoryRepository = new ClubHistoryRepository(dbContext);

                // Act
                await _clubHistoryRepository.CreateClubHistory(newClubHistory);

                // Assert
                var result = await dbContext.ClubHistories.FindAsync(3);
                Assert.NotNull(result);
                Assert.Equal("Inter Miami", result.ClubName);
                Assert.Equal("MLS", result.League);
            }
        }

        [Fact]
        public async Task UpdateClubHistory_UpdatesExistingClubHistory()
        {
            // Arrange
            var options = GetDbContextOptions("UpdateClubHistory_UpdatesExistingClubHistory");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedClubHistoryTestBase(dbContext);
                var _clubHistoryRepository = new ClubHistoryRepository(dbContext);

                var existingClubHistory = await dbContext.ClubHistories.FindAsync(1);
                existingClubHistory.ClubName = "Updated Club Name";
                existingClubHistory.League = "Updated League";

                // Act
                await _clubHistoryRepository.UpdateClubHistory(existingClubHistory);

                // Assert
                var result = await dbContext.ClubHistories.FindAsync(1);
                Assert.NotNull(result);
                Assert.Equal("Updated Club Name", result.ClubName);
                Assert.Equal("Updated League", result.League);
            }
        }

        [Fact]
        public async Task DeleteClubHistory_RemovesClubHistoryAndAchievements()
        {
            // Arrange
            var options = GetDbContextOptions("DeleteClubHistory_RemovesClubHistoryAndAchievements");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedClubHistoryTestBase(dbContext);
                var _clubHistoryRepository = new ClubHistoryRepository(dbContext);

                // Act
                await _clubHistoryRepository.DeleteClubHistory(1);

                // Assert
                var result = await dbContext.ClubHistories.FindAsync(1);
                Assert.Null(result);

                var relatedAchievements = await dbContext.Achievements.FindAsync(1);
                Assert.Null(relatedAchievements);
            }
        }
    }
}