using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using FootScout_PostgreSQL.WebAPI.UnitTests.TestManager;
using Microsoft.EntityFrameworkCore;

namespace FootScout_PostgreSQL.WebAPI.UnitTests.Repositories
{
    public class ClubOfferRepositoryTests : TestBase
    {
        [Fact]
        public async Task GetClubOffer_ReturnsCorrectClubOffer()
        {
            // Arrange
            var options = GetDbContextOptions("GetClubOffer_ReturnsCorrectClubOffer");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedPlayerAdvertisementTestBase(dbContext);
                await SeedClubOfferTestBase(dbContext);

                var _clubOfferRepository = new ClubOfferRepository(dbContext);

                // Act
                var result = await _clubOfferRepository.GetClubOffer(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal("Manchester City", result.ClubName);
                Assert.Equal("Premier League", result.League);
                Assert.Equal("England", result.Region);
            }
        }

        [Fact]
        public async Task GetClubOffers_ReturnsAllClubOffers()
        {
            // Arrange
            var options = GetDbContextOptions("GetClubOffers_ReturnsAllClubOffers");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedPlayerAdvertisementTestBase(dbContext);
                await SeedClubOfferTestBase(dbContext);

                var _clubOfferRepository = new ClubOfferRepository(dbContext);

                // Act
                var result = await _clubOfferRepository.GetClubOffers();

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                var firstOffer = result.First();
                Assert.Equal("Real Madrid", firstOffer.ClubName);
                Assert.Equal(2, result.ToList().Count);
            }
        }

        [Fact]
        public async Task GetActiveClubOffers_ReturnsActiveClubOffers()
        {
            // Arrange
            var options = GetDbContextOptions("GetActiveClubOffers_ReturnsActiveClubOffers");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedPlayerAdvertisementTestBase(dbContext);
                await SeedClubOfferTestBase(dbContext);

                var _clubOfferRepository = new ClubOfferRepository(dbContext);

                // Act
                var result = await _clubOfferRepository.GetActiveClubOffers();

                // Assert
                Assert.NotNull(result);
                Assert.All(result, co => Assert.True(co.PlayerAdvertisement.EndDate >= DateTime.Now));
            }
        }

        [Fact]
        public async Task GetActiveClubOfferCount_ReturnsCountOfActiveClubOffers()
        {
            // Arrange
            var options = GetDbContextOptions("GetActiveClubOfferCount_ReturnsCountOfActiveClubOffers");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedPlayerAdvertisementTestBase(dbContext);
                await SeedClubOfferTestBase(dbContext);

                var _clubOfferRepository = new ClubOfferRepository(dbContext);

                // Act
                var result = await _clubOfferRepository.GetActiveClubOfferCount();

                // Assert
                Assert.Equal(2, result);
            }
        }

        [Fact]
        public async Task GetInactiveClubOffers_ReturnsInactiveClubOffers()
        {
            // Arrange
            var options = GetDbContextOptions("GetInactiveClubOffers_ReturnsInactiveClubOffers");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedPlayerAdvertisementTestBase(dbContext);
                await SeedClubOfferTestBase(dbContext);

                var _clubOfferRepository = new ClubOfferRepository(dbContext);

                // Act
                var result = await _clubOfferRepository.GetInactiveClubOffers();

                // Assert
                Assert.NotNull(result);
                Assert.All(result, co => Assert.True(co.PlayerAdvertisement.EndDate < DateTime.Now));
            }
        }

        [Fact]
        public async Task CreateClubOffer_AddsClubOfferToDatabase()
        {
            // Arrange
            var options = GetDbContextOptions("CreateClubOffer_AddsClubOfferToDatabase");
            var newClubOffer = new ClubOffer
            {
                PlayerAdvertisementId = 1,
                PlayerPositionId = 5,
                ClubName = "New ClubName",
                League = "New League",
                Region = "New Region",
                Salary = 200,
                AdditionalInformation = "no info",
                CreationDate = DateTime.Now,
                ClubMemberId = "pepguardiola"
            };

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedPlayerAdvertisementTestBase(dbContext);
                await SeedClubOfferTestBase(dbContext);

                var _clubOfferRepository = new ClubOfferRepository(dbContext);

                // Act
                await _clubOfferRepository.CreateClubOffer(newClubOffer);

                // Assert
                var createdOffer = await dbContext.ClubOffers
                    .FirstOrDefaultAsync(co => co.ClubName == "New ClubName");
                Assert.NotNull(createdOffer);
                Assert.Equal("New ClubName", createdOffer.ClubName);
            }
        }

        [Fact]
        public async Task UpdateClubOffer_UpdatesClubOfferInDatabase()
        {
            // Arrange
            var options = GetDbContextOptions("UpdateClubOffer_UpdatesClubOfferInDatabase");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedPlayerAdvertisementTestBase(dbContext);
                await SeedClubOfferTestBase(dbContext);

                var _clubOfferRepository = new ClubOfferRepository(dbContext);
                var clubOffer = await dbContext.ClubOffers.FirstAsync();
                clubOffer.OfferStatusId = 2;

                // Act
                await _clubOfferRepository.UpdateClubOffer(clubOffer);

                // Assert
                var updatedOffer = await dbContext.ClubOffers
                    .FirstOrDefaultAsync(co => co.Id == clubOffer.Id);
                Assert.NotNull(updatedOffer);
                Assert.Equal(2, updatedOffer.OfferStatusId);
            }
        }
    }
}