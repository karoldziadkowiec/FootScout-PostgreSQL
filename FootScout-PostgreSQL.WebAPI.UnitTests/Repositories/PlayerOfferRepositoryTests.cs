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
                Assert.All(result, co => Assert.True(co.ClubAdvertisement.EndDate >= DateTime.UtcNow));
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
                Assert.All(result, co => Assert.True(co.ClubAdvertisement.EndDate < DateTime.UtcNow));
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
                PlayerPosition = new PlayerPosition { Id = 12, PositionName = "Left Midfield" },
                Age = 37,
                Height = 167,
                PlayerFootId = 1,
                PlayerFoot = new PlayerFoot { Id = 12, FootName = "Left" },
                Salary = 180,
                AdditionalInformation = "no info",
                CreationDate = DateTime.UtcNow,
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

        [Fact]
        public async Task DeletePlayerOffer_DeletesPlayerOfferFromDatabase()
        {
            // Arrange
            var options = GetDbContextOptions("DeletePlayerOffer_DeletesPlayerOfferFromDatabase");

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
                var playerOfferId = playerOffer.Id;

                // Act
                await _playerOfferRepository.DeletePlayerOffer(playerOfferId);

                // Assert
                var deletedOffer = await dbContext.PlayerOffers
                    .FirstOrDefaultAsync(po => po.Id == playerOfferId);

                Assert.Null(deletedOffer);
            }
        }

        [Fact]
        public async Task AcceptPlayerOffer_UpdatesOfferStatusToAccepted()
        {
            // Arrange
            var options = GetDbContextOptions("AcceptPlayerOffer_UpdatesOfferStatusToAccepted");

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

                // Act
                await _playerOfferRepository.AcceptPlayerOffer(playerOffer);

                // Assert
                var updatedOffer = await dbContext.PlayerOffers
                    .FirstOrDefaultAsync(po => po.Id == playerOffer.Id);

                Assert.NotNull(updatedOffer);
                Assert.Equal("Accepted", updatedOffer.OfferStatus.StatusName);
            }
        }

        [Fact]
        public async Task RejectPlayerOffer_UpdatesOfferStatusToRejected()
        {
            // Arrange
            var options = GetDbContextOptions("RejectPlayerOffer_UpdatesOfferStatusToRejected");

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

                // Act
                await _playerOfferRepository.RejectPlayerOffer(playerOffer);

                // Assert
                var updatedOffer = await dbContext.PlayerOffers
                    .FirstOrDefaultAsync(po => po.Id == playerOffer.Id);

                Assert.NotNull(updatedOffer);
                Assert.Equal("Rejected", updatedOffer.OfferStatus.StatusName);
            }
        }

        [Fact]
        public async Task GetPlayerOfferStatusId_ReturnsCorrectStatusId()
        {
            // Arrange
            var options = GetDbContextOptions("GetPlayerOfferStatusId_ReturnsCorrectStatusId");

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
                var clubAdvertisementId = playerOffer.ClubAdvertisementId;
                var userId = playerOffer.PlayerId;

                // Act
                var result = await _playerOfferRepository.GetPlayerOfferStatusId(clubAdvertisementId, userId);

                // Assert
                Assert.Equal(playerOffer.OfferStatusId, result);
            }
        }

        [Fact]
        public async Task ExportPlayerOffersToCsv_ReturnsValidCsvStream()
        {
            // Arrange
            var options = GetDbContextOptions("ExportPlayerOffersToCsv_ReturnsValidCsvStream");

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
                var csvStream = await _playerOfferRepository.ExportPlayerOffersToCsv();
                csvStream.Position = 0;

                using (var reader = new StreamReader(csvStream))
                {
                    var csvContent = await reader.ReadToEndAsync();

                    // Assert
                    Assert.NotEmpty(csvContent);
                    Assert.Contains("Offer Status,E-mail,First Name,Last Name,Position,Age,Height,Foot,Salary,Additional Information,Club Member's E-mail,Club Member's First Name,Club Member's Last Name,Club Name,League,Region,Creation Date,End Date", csvContent);
                    Assert.Contains("Offered,lm10@gmail.com,Leo", csvContent);
                }
            }
        }
    }
}