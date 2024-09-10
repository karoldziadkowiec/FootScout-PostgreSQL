using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using FootScout_PostgreSQL.WebAPI.UnitTests.TestManager;
using Microsoft.EntityFrameworkCore;

namespace FootScout_PostgreSQL.WebAPI.UnitTests.Repositories
{
    public class PlayerOfferRepositoryTests : TestBase
    {
        [Fact]
        public async Task GetPlayerOffer_ReturnsCorrectPlayerOffer()
        {
            // Arrange
            var options = GetDbContextOptions("GetPlayerOffer_ReturnsCorrectPlayerOffer");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedClubAdvertisementTestBase(dbContext);
                await SeedPlayerOfferTestBase(dbContext);

                var _playerOfferRepository = new PlayerOfferRepository(dbContext);

                // Act
                var result = await _playerOfferRepository.GetPlayerOffer(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal("lm10@gmail.com", result.Player.Email);
                Assert.Equal(1, result.PlayerFootId);
            }
        }

        [Fact]
        public async Task GetPlayerOffers_ReturnsAllPlayerOffers()
        {
            // Arrange
            var options = GetDbContextOptions("GetPlayerOffers_ReturnsAllPlayerOffers");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedClubAdvertisementTestBase(dbContext);
                await SeedPlayerOfferTestBase(dbContext);

                var _playerOfferRepository = new PlayerOfferRepository(dbContext);

                // Act
                var result = await _playerOfferRepository.GetPlayerOffers();

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                var firstOffer = result.First();
                Assert.Equal(14, firstOffer.PlayerPositionId);
                Assert.Equal(2, result.ToList().Count);
            }
        }

        [Fact]
        public async Task GetActivePlayerOffers_ReturnsActivePlayerOffers()
        {
            // Arrange
            var options = GetDbContextOptions("GetActivePlayerOffers_ReturnsActivePlayerOffers");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedClubAdvertisementTestBase(dbContext);
                await SeedPlayerOfferTestBase(dbContext);

                var _playerOfferRepository = new PlayerOfferRepository(dbContext);

                // Act
                var result = await _playerOfferRepository.GetActivePlayerOffers();

                // Assert
                Assert.NotNull(result);
                Assert.All(result, co => Assert.True(co.ClubAdvertisement.EndDate >= DateTime.Now));
            }
        }

        [Fact]
        public async Task GetActivePlayerOfferCount_ReturnsCountOfActivePlayerOffers()
        {
            // Arrange
            var options = GetDbContextOptions("GetActivePlayerOfferCount_ReturnsCountOfActivePlayerOffers");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedClubAdvertisementTestBase(dbContext);
                await SeedPlayerOfferTestBase(dbContext);

                var _playerOfferRepository = new PlayerOfferRepository(dbContext);

                // Act
                var result = await _playerOfferRepository.GetActivePlayerOfferCount();

                // Assert
                Assert.Equal(2, result);
            }
        }

        [Fact]
        public async Task GetInactivePlayerOffers_ReturnsInactivePlayerOffers()
        {
            // Arrange
            var options = GetDbContextOptions("GetInactivePlayerOffers_ReturnsInactivePlayerOffers");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedClubAdvertisementTestBase(dbContext);
                await SeedPlayerOfferTestBase(dbContext);

                var _playerOfferRepository = new PlayerOfferRepository(dbContext);

                // Act
                var result = await _playerOfferRepository.GetInactivePlayerOffers();

                // Assert
                Assert.NotNull(result);
                Assert.All(result, co => Assert.True(co.ClubAdvertisement.EndDate < DateTime.Now));
            }
        }

        [Fact]
        public async Task CreatePlayerOffer_AddsPlayerOfferToDatabase()
        {
            // Arrange
            var options = GetDbContextOptions("CreatePlayerOffer_AddsPlayerOfferToDatabase");
            var newPlayerOffer = new PlayerOffer
            {
                ClubAdvertisementId = 2,
                PlayerPositionId = 12,
                Age = 37,
                Height = 167,
                PlayerFootId = 1,
                Salary = 180,
                AdditionalInformation = "no info",
                CreationDate = DateTime.Now,
                PlayerId = "leomessi"
            };

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedClubAdvertisementTestBase(dbContext);
                await SeedPlayerOfferTestBase(dbContext);

                var _playerOfferRepository = new PlayerOfferRepository(dbContext);

                // Act
                await _playerOfferRepository.CreatePlayerOffer(newPlayerOffer);

                // Assert
                var createdOffer = await dbContext.PlayerOffers
                    .FirstOrDefaultAsync(co => co.PlayerPositionId == 12);
                Assert.NotNull(createdOffer);
                Assert.Equal("leomessi", createdOffer.Player.Id);
                Assert.Equal(12, createdOffer.PlayerPositionId);
            }
        }

        [Fact]
        public async Task UpdatePlayerOffer_UpdatesPlayerOfferInDatabase()
        {
            // Arrange
            var options = GetDbContextOptions("UpdatePlayerOffer_UpdatesPlayerOfferInDatabase");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedClubAdvertisementTestBase(dbContext);
                await SeedPlayerOfferTestBase(dbContext);

                var _playerOfferRepository = new PlayerOfferRepository(dbContext);
                var playerOffer = await dbContext.PlayerOffers.FirstAsync();
                playerOffer.OfferStatusId = 2;

                // Act
                await _playerOfferRepository.UpdatePlayerOffer(playerOffer);

                // Assert
                var updatedOffer = await dbContext.PlayerOffers
                    .FirstOrDefaultAsync(co => co.Id == playerOffer.Id);
                Assert.NotNull(updatedOffer);
                Assert.Equal(2, updatedOffer.OfferStatusId);
            }
        }
    }
}