using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Services.Classes;
using FootScout_PostgreSQL.WebAPI.UnitTests.TestManager;
using Microsoft.EntityFrameworkCore;

namespace FootScout_PostgreSQL.WebAPI.UnitTests.Services
{
    public class PerformanceTestsServiceTests : TestBase
    {
        [Fact]
        public async Task SeedComponents_AddsNewComponents()
        {
            // Arrange
            var options = GetDbContextOptions("SeedComponents_AddsNewComponents");
            var testCounter = 10;

            using (var dbContext = new AppDbContext(options))
            {
                await SeedRoleTestBase(dbContext);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                var _performanceTestsService = new PerformanceTestsService(dbContext);

                // Act
                await _performanceTestsService.SeedComponents(testCounter);

                // Assert
                var usersCount = await dbContext.Users.CountAsync();
                Assert.Equal(testCounter, usersCount);

                var userRolesCount = await dbContext.UserRoles.CountAsync();
                Assert.Equal(testCounter, userRolesCount);

                var achievementsCount = await dbContext.Achievements.CountAsync();
                Assert.Equal(testCounter, achievementsCount);

                var clubHistoriesCount = await dbContext.ClubHistories.CountAsync();
                Assert.Equal(testCounter, clubHistoriesCount);

                var problemsCount = await dbContext.Problems.CountAsync();
                Assert.Equal(testCounter, problemsCount);

                var chatsCount = await dbContext.Chats.CountAsync();
                Assert.Equal(testCounter, chatsCount);

                var messagesCount = await dbContext.Messages.CountAsync();
                Assert.Equal(testCounter, messagesCount);

                var playerAdvertisementsCount = await dbContext.PlayerAdvertisements.CountAsync();
                Assert.Equal(testCounter, playerAdvertisementsCount);

                var favoritePlayerAdvertisementsCount = await dbContext.FavoritePlayerAdvertisements.CountAsync();
                Assert.Equal(testCounter, favoritePlayerAdvertisementsCount);

                var clubOffersCount = await dbContext.ClubOffers.CountAsync();
                Assert.Equal(testCounter, clubOffersCount);

                var clubAdvertisementsCount = await dbContext.ClubAdvertisements.CountAsync();
                Assert.Equal(testCounter, clubAdvertisementsCount);

                var favoriteClubAdvertisementsCount = await dbContext.FavoriteClubAdvertisements.CountAsync();
                Assert.Equal(testCounter, favoriteClubAdvertisementsCount);

                var playerOffersCount = await dbContext.PlayerOffers.CountAsync();
                Assert.Equal(testCounter, playerOffersCount);

                var salaryRangesCount = await dbContext.SalaryRanges.CountAsync();
                Assert.Equal(2 * testCounter, salaryRangesCount);
            }
        }

        [Fact]
        public async Task ClearDatabaseOfSeededComponents_RemovesSeededComponents()
        {
            // Arrange
            var options = GetDbContextOptions("ClearDatabaseOfSeededComponents_RemovesSeededComponents");
            var expectedValue = 0;

            using (var dbContext = new AppDbContext(options))
            {
                await SeedRoleTestBase(dbContext);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                var _performanceTestsService = new PerformanceTestsService(dbContext);

                // Act
                await _performanceTestsService.ClearDatabaseOfSeededComponents();

                // Assert
                var usersCount = await dbContext.Users.CountAsync();
                Assert.Equal(expectedValue, usersCount);

                var userRolesCount = await dbContext.UserRoles.CountAsync();
                Assert.Equal(expectedValue, userRolesCount);

                var achievementsCount = await dbContext.Achievements.CountAsync();
                Assert.Equal(expectedValue, achievementsCount);

                var clubHistoriesCount = await dbContext.ClubHistories.CountAsync();
                Assert.Equal(expectedValue, clubHistoriesCount);

                var problemsCount = await dbContext.Problems.CountAsync();
                Assert.Equal(expectedValue, problemsCount);

                var chatsCount = await dbContext.Chats.CountAsync();
                Assert.Equal(expectedValue, chatsCount);

                var messagesCount = await dbContext.Messages.CountAsync();
                Assert.Equal(expectedValue, messagesCount);

                var playerAdvertisementsCount = await dbContext.PlayerAdvertisements.CountAsync();
                Assert.Equal(expectedValue, playerAdvertisementsCount);

                var favoritePlayerAdvertisementsCount = await dbContext.FavoritePlayerAdvertisements.CountAsync();
                Assert.Equal(expectedValue, favoritePlayerAdvertisementsCount);

                var clubOffersCount = await dbContext.ClubOffers.CountAsync();
                Assert.Equal(expectedValue, clubOffersCount);

                var clubAdvertisementsCount = await dbContext.ClubAdvertisements.CountAsync();
                Assert.Equal(expectedValue, clubAdvertisementsCount);

                var favoriteClubAdvertisementsCount = await dbContext.FavoriteClubAdvertisements.CountAsync();
                Assert.Equal(expectedValue, favoriteClubAdvertisementsCount);

                var playerOffersCount = await dbContext.PlayerOffers.CountAsync();
                Assert.Equal(expectedValue, playerOffersCount);

                var salaryRangesCount = await dbContext.SalaryRanges.CountAsync();
                Assert.Equal(expectedValue, salaryRangesCount);
            }
        }
    }
}