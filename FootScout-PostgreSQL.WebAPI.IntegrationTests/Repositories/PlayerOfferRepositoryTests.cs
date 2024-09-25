using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using Microsoft.EntityFrameworkCore;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Repositories
{
    public class PlayerOfferRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly AppDbContext _dbContext;
        private readonly PlayerOfferRepository _playerOfferRepository;

        public PlayerOfferRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _playerOfferRepository = new PlayerOfferRepository(_dbContext);
        }

        [Fact]
        public async Task GetPlayerOffer_ReturnsCorrectPlayerOffer()
        {
            // Arrange & Act
            var result = await _playerOfferRepository.GetPlayerOffer(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("lm10@gmail.com", result.Player.Email);
            Assert.Equal(1, result.PlayerFootId);
        }

        [Fact]
        public async Task GetPlayerOffers_ReturnsAllPlayerOffers()
        {
            // Arrange & Act
            var result = await _playerOfferRepository.GetPlayerOffers();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            var firstOffer = result.First();
            Assert.Equal(14, firstOffer.PlayerPositionId);
            Assert.Equal(2, result.ToList().Count);
        }

        [Fact]
        public async Task GetActivePlayerOffers_ReturnsActivePlayerOffers()
        {
            // Arrange & Act
            var result = await _playerOfferRepository.GetActivePlayerOffers();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, po => Assert.True(po.ClubAdvertisement.EndDate >= DateTime.UtcNow));
        }

        [Fact]
        public async Task GetActivePlayerOfferCount_ReturnsCountOfActivePlayerOffers()
        {
            // Arrange &  Act
            var result = await _playerOfferRepository.GetActivePlayerOfferCount();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetInactivePlayerOffers_ReturnsInactivePlayerOffers()
        {
            // Arrange & Act
            var result = await _playerOfferRepository.GetInactivePlayerOffers();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, po => Assert.True(po.ClubAdvertisement.EndDate < DateTime.UtcNow));
        }

        [Fact]
        public async Task CreatePlayerOffer_AddsPlayerOfferToDatabase()
        {
            // Arrange
            var newPlayerOffer = new PlayerOffer
            {
                ClubAdvertisementId = 2,
                PlayerPositionId = 15,
                PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" },
                Age = 37,
                Height = 167,
                PlayerFootId = 1,
                PlayerFoot = new PlayerFoot { Id = 1, FootName = "Left" },
                Salary = 180,
                AdditionalInformation = "no info",
                CreationDate = DateTime.UtcNow,
                PlayerId = "leomessi"
            };

            // Act
            await _playerOfferRepository.CreatePlayerOffer(newPlayerOffer);

            var result = await _dbContext.PlayerOffers
                .FirstOrDefaultAsync(co => co.PlayerPositionId == 15);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("leomessi", result.Player.Id);
            Assert.Equal(15, result.PlayerPositionId);

            _dbContext.PlayerOffers.Remove(result);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdatePlayerOffer_UpdatesPlayerOfferInDatabase()
        {
            // Arrange
            var playerOffer = await _dbContext.PlayerOffers.FirstAsync();
            playerOffer.OfferStatusId = 2;

            // Act
            await _playerOfferRepository.UpdatePlayerOffer(playerOffer);

            // Assert
            var updatedOffer = await _dbContext.PlayerOffers
                .FirstOrDefaultAsync(po => po.Id == playerOffer.Id);

            Assert.NotNull(updatedOffer);
            Assert.Equal(2, updatedOffer.OfferStatusId);
        }

        [Fact]
        public async Task DeletePlayerOffer_DeletesPlayerOfferFromDatabase()
        {
            // Arrange
            _dbContext.PlayerOffers.Add(new PlayerOffer
            {
                ClubAdvertisementId = 2,
                OfferStatusId = 1,
                OfferStatus = new OfferStatus { Id = 1, StatusName = "Offered" },
                PlayerPositionId = 15,
                PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" },
                Age = 37,
                Height = 168,
                PlayerFootId = 1,
                PlayerFoot = new PlayerFoot { Id = 1, FootName = "Left" },
                Salary = 280,
                AdditionalInformation = "no info",
                CreationDate = DateTime.UtcNow,
                PlayerId = "leomessi"
            });
            await _dbContext.SaveChangesAsync();

            var offerToDelete = await _dbContext.PlayerOffers
                .FirstOrDefaultAsync(po => po.ClubAdvertisementId == 2 && po.PlayerPositionId == 15 && po.Height == 168 && po.Salary == 280 && po.PlayerId == "leomessi");

            // Act
            await _playerOfferRepository.DeletePlayerOffer(offerToDelete.Id);

            // Assert
            var deletedOffer = await _dbContext.PlayerOffers
                .FirstOrDefaultAsync(po => po.Id == offerToDelete.Id);

            Assert.Null(deletedOffer);
        }

        [Fact]
        public async Task AcceptPlayerOffer_UpdatesOfferStatusToAccepted()
        {
            // Arrange
            var playerOffer = await _dbContext.PlayerOffers.FirstAsync();

            // Act
            await _playerOfferRepository.AcceptPlayerOffer(playerOffer);

            // Assert
            var updatedOffer = await _dbContext.PlayerOffers
                .FirstOrDefaultAsync(po => po.Id == playerOffer.Id);

            Assert.NotNull(updatedOffer);
            Assert.Equal("Accepted", updatedOffer.OfferStatus.StatusName);
        }

        [Fact]
        public async Task RejectPlayerOffer_UpdatesOfferStatusToRejected()
        {
            // Arrange
            var playerOffer = await _dbContext.PlayerOffers.FirstAsync();

            // Act
            await _playerOfferRepository.RejectPlayerOffer(playerOffer);

            // Assert
            var updatedOffer = await _dbContext.PlayerOffers
                .FirstOrDefaultAsync(po => po.Id == playerOffer.Id);

            Assert.NotNull(updatedOffer);
            Assert.Equal("Rejected", updatedOffer.OfferStatus.StatusName);
        }

        [Fact]
        public async Task GetPlayerOfferStatusId_ReturnsCorrectStatusId()
        {
            // Arrange
            var playerOffer = await _dbContext.PlayerOffers.FirstAsync();
            var clubAdvertisementId = playerOffer.ClubAdvertisementId;
            var playerId = playerOffer.PlayerId;

            // Act
            var result = await _playerOfferRepository.GetPlayerOfferStatusId(clubAdvertisementId, playerId);

            // Assert
            Assert.Equal(playerOffer.OfferStatusId, result);
        }

        [Fact]
        public async Task ExportPlayerOffersToCsv_ReturnsValidCsvStream()
        {
            // Arrange && Act
            var csvStream = await _playerOfferRepository.ExportPlayerOffersToCsv();
            csvStream.Position = 0;

            using (var reader = new StreamReader(csvStream))
            {
                var csvContent = await reader.ReadToEndAsync();

                // Assert
                Assert.NotEmpty(csvContent);
                Assert.Contains("Offer Status,E-mail,First Name,Last Name,Position,Age,Height,Foot,Salary,Additional Information,Club Member's E-mail,Club Member's First Name,Club Member's Last Name,Club Name,League,Region,Creation Date,End Date", csvContent);
                Assert.Contains("lm10@gmail.com,Leo,Messi", csvContent);
            }
        }
    }
}