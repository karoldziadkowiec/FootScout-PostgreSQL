using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using FootScout_PostgreSQL.WebAPI.Services.Classes;
using Microsoft.EntityFrameworkCore;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Services
{
    public class PerformanceTestsServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly AppDbContext _dbContext;
        private readonly PerformanceTestsService _performanceTestsService;

        public PerformanceTestsServiceTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _performanceTestsService = new PerformanceTestsService(_dbContext);
        }

        [Fact]
        public async Task SeedComponents_AddsNewComponents()
        {
            // Arrange
            var testCounter = 10;

            // Act
            await _performanceTestsService.SeedComponents(testCounter);

            // Assert
            var usersCount = await _dbContext.Users.CountAsync();
            Assert.True(usersCount >= testCounter);

            var userRolesCount = await _dbContext.UserRoles.CountAsync();
            Assert.True(userRolesCount >= testCounter);

            var achievementsCount = await _dbContext.Achievements.CountAsync();
            Assert.True(achievementsCount >= testCounter);

            var clubHistoriesCount = await _dbContext.ClubHistories.CountAsync();
            Assert.True(clubHistoriesCount >= testCounter);

            var problemsCount = await _dbContext.Problems.CountAsync();
            Assert.True(problemsCount >= testCounter);

            var chatsCount = await _dbContext.Chats.CountAsync();
            Assert.True(chatsCount >= testCounter);

            var messagesCount = await _dbContext.Messages.CountAsync();
            Assert.True(messagesCount >= testCounter);

            var playerAdvertisementsCount = await _dbContext.PlayerAdvertisements.CountAsync();
            Assert.True(playerAdvertisementsCount >= testCounter);

            var favoritePlayerAdvertisementsCount = await _dbContext.FavoritePlayerAdvertisements.CountAsync();
            Assert.True(favoritePlayerAdvertisementsCount >= testCounter);

            var clubOffersCount = await _dbContext.ClubOffers.CountAsync();
            Assert.True(clubOffersCount >= testCounter);

            var clubAdvertisementsCount = await _dbContext.ClubAdvertisements.CountAsync();
            Assert.True(clubAdvertisementsCount >= testCounter);

            var favoriteClubAdvertisementsCount = await _dbContext.FavoriteClubAdvertisements.CountAsync();
            Assert.True(favoriteClubAdvertisementsCount >= testCounter);

            var playerOffersCount = await _dbContext.PlayerOffers.CountAsync();
            Assert.True(playerOffersCount >= testCounter);

            var salaryRangesCount = await _dbContext.SalaryRanges.CountAsync();
            Assert.True(salaryRangesCount >= 2 * testCounter);

            await _performanceTestsService.ClearDatabaseOfSeededComponents();
        }

        [Fact]
        public async Task ClearDatabaseOfSeededComponents_RemovesDatabaseOfSeededComponents()
        {
            // Arrange
            var expectedValue = 0;

            // Act
            await _performanceTestsService.ClearDatabaseOfSeededComponents();

            // Assert
            var usersCount = await _dbContext.Users.CountAsync();
            Assert.Equal(2, usersCount);

            var userRolesCount = await _dbContext.UserRoles.CountAsync();
            Assert.Equal(2, userRolesCount);

            var achievementsCount = await _dbContext.Achievements.CountAsync();
            Assert.Equal(expectedValue, achievementsCount);

            var clubHistoriesCount = await _dbContext.ClubHistories.CountAsync();
            Assert.Equal(expectedValue, clubHistoriesCount);

            var problemsCount = await _dbContext.Problems.CountAsync();
            Assert.Equal(expectedValue, problemsCount);

            var chatsCount = await _dbContext.Chats.CountAsync();
            Assert.Equal(expectedValue, chatsCount);

            var messagesCount = await _dbContext.Messages.CountAsync();
            Assert.Equal(expectedValue, messagesCount);

            var playerAdvertisementsCount = await _dbContext.PlayerAdvertisements.CountAsync();
            Assert.Equal(expectedValue, playerAdvertisementsCount);

            var favoritePlayerAdvertisementsCount = await _dbContext.FavoritePlayerAdvertisements.CountAsync();
            Assert.Equal(expectedValue, favoritePlayerAdvertisementsCount);

            var clubOffersCount = await _dbContext.ClubOffers.CountAsync();
            Assert.Equal(expectedValue, clubOffersCount);

            var clubAdvertisementsCount = await _dbContext.ClubAdvertisements.CountAsync();
            Assert.Equal(expectedValue, clubAdvertisementsCount);

            var favoriteClubAdvertisementsCount = await _dbContext.FavoriteClubAdvertisements.CountAsync();
            Assert.Equal(expectedValue, favoriteClubAdvertisementsCount);

            var playerOffersCount = await _dbContext.PlayerOffers.CountAsync();
            Assert.Equal(expectedValue, playerOffersCount);

            var salaryRangesCount = await _dbContext.SalaryRanges.CountAsync();
            Assert.Equal(expectedValue, salaryRangesCount);
        }
    }
}