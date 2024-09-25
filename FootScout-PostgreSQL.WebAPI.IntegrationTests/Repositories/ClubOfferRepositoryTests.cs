using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using Microsoft.EntityFrameworkCore;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Repositories
{
    public class ClubOfferRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly AppDbContext _dbContext;
        private readonly ClubOfferRepository _clubOfferRepository;

        public ClubOfferRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _clubOfferRepository = new ClubOfferRepository(_dbContext);
        }

        [Fact]
        public async Task GetClubOffer_ReturnsCorrectClubOffer()
        {
            // Arrange & Act
            var result = await _clubOfferRepository.GetClubOffer(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Manchester City", result.ClubName);
            Assert.Equal("Premier League", result.League);
            Assert.Equal("England", result.Region);
        }

        [Fact]
        public async Task GetClubOffers_ReturnsAllClubOffers()
        {
            // Arrange & Act
            var result = await _clubOfferRepository.GetClubOffers();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            var firstOffer = result.First();
            Assert.Equal("Real Madrid", firstOffer.ClubName);
            Assert.Equal(2, result.ToList().Count);
        }

        [Fact]
        public async Task GetActiveClubOffers_ReturnsActiveClubOffers()
        {
            // Arrange & Act
            var result = await _clubOfferRepository.GetActiveClubOffers();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, co => Assert.True(co.PlayerAdvertisement.EndDate >= DateTime.UtcNow));
        }

        [Fact]
        public async Task GetActiveClubOfferCount_ReturnsCountOfActiveClubOffers()
        {
            // Arrange &  Act
            var result = await _clubOfferRepository.GetActiveClubOfferCount();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetInactiveClubOffers_ReturnsInactiveClubOffers()
        {
            // Arrange & Act
            var result = await _clubOfferRepository.GetInactiveClubOffers();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, co => Assert.True(co.PlayerAdvertisement.EndDate < DateTime.UtcNow));
        }

        [Fact]
        public async Task CreateClubOffer_AddsClubOfferToDatabase()
        {
            // Arrange
            var newClubOffer = new ClubOffer
            {
                PlayerAdvertisementId = 1,
                PlayerPositionId = 15,
                PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" },
                ClubName = "New ClubName",
                League = "New League",
                Region = "New Region",
                Salary = 200,
                AdditionalInformation = "no info",
                CreationDate = DateTime.UtcNow,
                ClubMemberId = "pepguardiola"
            };

            // Act
            await _clubOfferRepository.CreateClubOffer(newClubOffer);

            var result = await _dbContext.ClubOffers
                .FirstOrDefaultAsync(co => co.ClubName == "New ClubName");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New ClubName", result.ClubName);

            _dbContext.ClubOffers.Remove(result);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateClubOffer_UpdatesClubOfferInDatabase()
        {
            // Arrange
            var clubOffer = await _dbContext.ClubOffers.FirstAsync();
            clubOffer.OfferStatusId = 2;

            // Act
            await _clubOfferRepository.UpdateClubOffer(clubOffer);

            // Assert
            var updatedOffer = await _dbContext.ClubOffers
                .FirstOrDefaultAsync(co => co.Id == clubOffer.Id);

            Assert.NotNull(updatedOffer);
            Assert.Equal(2, updatedOffer.OfferStatusId);
        }

        [Fact]
        public async Task DeleteClubOffer_DeletesClubOfferFromDatabase()
        {
            // Arrange
            _dbContext.ClubOffers.Add(new ClubOffer
            {
                PlayerAdvertisementId = 1,
                OfferStatusId = 1,
                OfferStatus = new OfferStatus { Id = 1, StatusName = "Offered" },
                PlayerPositionId = 15,
                PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" },
                ClubName = "Juventus Turyn",
                League = "Serie A",
                Region = "Italy",
                Salary = 260,
                AdditionalInformation = "no info",
                CreationDate = DateTime.UtcNow,
                ClubMemberId = "pepguardiola"
            });
            await _dbContext.SaveChangesAsync();

            var offerToDelete = await _dbContext.ClubOffers
                .FirstOrDefaultAsync(co => co.PlayerAdvertisementId == 1 && co.ClubName == "Juventus Turyn" && co.League == "Serie A" && co.Region == "Italy" && co.ClubMemberId == "pepguardiola");

            // Act
            await _clubOfferRepository.DeleteClubOffer(offerToDelete.Id);

            // Assert
            var deletedOffer = await _dbContext.ClubOffers
                .FirstOrDefaultAsync(co => co.Id == offerToDelete.Id);

            Assert.Null(deletedOffer);
        }

        [Fact]
        public async Task AcceptClubOffer_UpdatesOfferStatusToAccepted()
        {
            // Arrange
            var clubOffer = await _dbContext.ClubOffers.FirstAsync();

            // Act
            await _clubOfferRepository.AcceptClubOffer(clubOffer);

            // Assert
            var updatedOffer = await _dbContext.ClubOffers
                .FirstOrDefaultAsync(co => co.Id == clubOffer.Id);

            Assert.NotNull(updatedOffer);
            Assert.Equal("Accepted", updatedOffer.OfferStatus.StatusName);
        }

        [Fact]
        public async Task RejectClubOffer_UpdatesOfferStatusToRejected()
        {
            // Arrange
            var clubOffer = await _dbContext.ClubOffers.FirstAsync();

            // Act
            await _clubOfferRepository.RejectClubOffer(clubOffer);

            // Assert
            var updatedOffer = await _dbContext.ClubOffers
                .FirstOrDefaultAsync(co => co.Id == clubOffer.Id);

            Assert.NotNull(updatedOffer);
            Assert.Equal("Rejected", updatedOffer.OfferStatus.StatusName);
        }

        [Fact]
        public async Task GetClubOfferStatusId_ReturnsCorrectStatusId()
        {
            // Arrange
            var clubOffer = await _dbContext.ClubOffers.FirstAsync();
            var playerAdvertisementId = clubOffer.PlayerAdvertisementId;
            var clubMemberId = clubOffer.ClubMemberId;

            // Act
            var result = await _clubOfferRepository.GetClubOfferStatusId(playerAdvertisementId, clubMemberId);

            // Assert
            Assert.Equal(clubOffer.OfferStatusId, result);
        }

        [Fact]
        public async Task ExportClubOffersToCsv_ReturnsValidCsvStream()
        {
            // Arrange && Act
            var csvStream = await _clubOfferRepository.ExportClubOffersToCsv();
            csvStream.Position = 0;

            using (var reader = new StreamReader(csvStream))
            {
                var csvContent = await reader.ReadToEndAsync();

                // Assert
                Assert.NotEmpty(csvContent);
                Assert.Contains("Offer Status,E-mail,First Name,Last Name,Position,Club Name,League,Region,Salary,Additional Information,Player's E-mail,Player's First Name,Player's Last Name,Age,Height,Foot,Creation Date,End Date", csvContent);
                Assert.Contains("pg8@gmail.com,Pep,Guardiola", csvContent);
            }
        }
    }
}