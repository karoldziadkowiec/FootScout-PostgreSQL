using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using FootScout_PostgreSQL.WebAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using Microsoft.EntityFrameworkCore;
using FootScout_PostgreSQL.WebAPI.DbManager;
using Microsoft.Extensions.DependencyInjection;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Controllers
{
    public class PlayerOfferControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public PlayerOfferControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
        {
            _fixture = fixture;
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // new DbContext using DatabaseFixture
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseNpgsql(_fixture.DbContext.Database.GetDbConnection().ConnectionString);
                    });
                });
            }).CreateClient();

            var userTokenJWT = _fixture.UserTokenJWT;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userTokenJWT);
        }

        [Fact]
        public async Task GetPlayerOffer_ReturnsOk_WhenPlayerOfferExists()
        {
            // Arrange
            var playerOfferId = 1;

            // Act
            var response = await _client.GetAsync($"/api/player-offers/{playerOfferId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var playerOffer = await response.Content.ReadFromJsonAsync<PlayerOffer>();
            Assert.Equal(playerOfferId, playerOffer.Id);
        }

        [Fact]
        public async Task GetPlayerOffer_ReturnsNotFound_WhenPlayerOfferDoesNotExist()
        {
            // Arrange
            var playerOfferId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/player-offers/{playerOfferId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllPlayerOffers_ReturnsOk_WhenPlayerOffersExists()
        {
            // Arrange & Act
            var response = await _client.GetAsync($"/api/player-offers");

            // Assert
            response.EnsureSuccessStatusCode();
            var playerOffers = await response.Content.ReadFromJsonAsync<IEnumerable<PlayerOffer>>();
            Assert.NotEmpty(playerOffers);
        }

        [Fact]
        public async Task GetActivePlayerOffers_ReturnsOk_WhenActivePlayerOffersExist()
        {
            // Act
            var response = await _client.GetAsync("/api/player-offers/active");

            // Assert
            response.EnsureSuccessStatusCode();
            var activePlayerOffers = await response.Content.ReadFromJsonAsync<IEnumerable<PlayerOffer>>();
            Assert.NotEmpty(activePlayerOffers);
        }

        [Fact]
        public async Task GetActivePlayerOffersCount_ReturnsOk_WithCorrectCount()
        {
            // Arrange
            var expectedCount = 2;

            // Act
            var response = await _client.GetAsync("/api/player-offers/active/count");

            // Assert
            response.EnsureSuccessStatusCode();
            var countString = await response.Content.ReadAsStringAsync();
            int actualCount = int.Parse(countString);
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetInactivePlayerOffers_ReturnsOk_WhenInactivePlayerOffersExist()
        {
            // Act
            var response = await _client.GetAsync("/api/player-offers/inactive");

            // Assert
            response.EnsureSuccessStatusCode();
            var inactivePlayerOffers = await response.Content.ReadFromJsonAsync<IEnumerable<PlayerOffer>>();
            Assert.Empty(inactivePlayerOffers);
        }

        [Fact]
        public async Task CreatePlayerOffer_ReturnsOk_WhenDataIsValid()
        {
            // Arrange
            var adDto = new PlayerOfferCreateDTO
            {
                ClubAdvertisementId = 1,
                PlayerPositionId = 12,
                Age = 37,
                Height = 167,
                PlayerFootId = 1,
                Salary = 232,
                AdditionalInformation = "no info",
                PlayerId = "leomessi"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/player-offers", adDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PlayerOffer>();
            Assert.NotNull(result);
            Assert.Equal(adDto.PlayerPositionId, result.PlayerPositionId);
            Assert.Equal(adDto.Age, result.Age);
            Assert.Equal(adDto.Height, result.Height);
            Assert.Equal(adDto.PlayerFootId, result.PlayerFootId);
            Assert.Equal(adDto.Salary, result.Salary);
            Assert.Equal(adDto.PlayerId, result.PlayerId);

            var clubOffer = await _fixture.DbContext.PlayerOffers.FirstOrDefaultAsync(po => po.PlayerPositionId == adDto.PlayerPositionId && po.Age == adDto.Age && po.Height == adDto.Height && po.PlayerFootId == adDto.PlayerFootId && po.Salary == adDto.Salary && po.PlayerId == adDto.PlayerId);
            _fixture.DbContext.PlayerOffers.Remove(clubOffer);
            await _fixture.DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task CreatePlayerOffer_ReturnsBadRequest_WhenDtoIsInvalid()
        {
            // Arrange
            PlayerOfferCreateDTO invalidDto = null;

            // Act
            var response = await _client.PostAsJsonAsync("/api/player-offers", invalidDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorMessage = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task UpdatePlayerOffer_ReturnsNoContent_WhenPlayerOfferExists()
        {
            // Arrange
            var offerToUpdate = await _fixture.DbContext.PlayerOffers.FirstAsync();
            offerToUpdate.Height = 168;

            // Act
            var response = await _client.PutAsJsonAsync($"/api/player-offers/{offerToUpdate.Id}", offerToUpdate);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePlayerOffer_ReturnsNotFound_WhenPlayerOfferDoesNotExist()
        {
            // Arrange
            var clubAdvertisementId = 2;
            var advertisementToUpdate = await _fixture.DbContext.PlayerOffers.FirstAsync();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/player-offers/{clubAdvertisementId}", advertisementToUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeletePlayerOffer_RemovesPlayerOffer()
        {
            // Arrange
            var newAd = new PlayerOfferCreateDTO
            {
                ClubAdvertisementId = 1,
                PlayerPositionId = 12,
                Age = 37,
                Height = 167,
                PlayerFootId = 1,
                Salary = 233,
                AdditionalInformation = "no info",
                PlayerId = "leomessi"
            };
            var response = await _client.PostAsJsonAsync("/api/player-offers", newAd);
            var clubOffer = await _fixture.DbContext.PlayerOffers.FirstOrDefaultAsync(po => po.PlayerPositionId == newAd.PlayerPositionId && po.Age == newAd.Age && po.Height == newAd.Height && po.PlayerFootId == newAd.PlayerFootId && po.Salary == newAd.Salary && po.PlayerId == newAd.PlayerId);

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/player-offers/{clubOffer.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task AcceptPlayerOffer_ReturnsNoContent_WhenPlayerOfferExists()
        {
            // Arrange
            var newAd = new PlayerOfferCreateDTO
            {
                ClubAdvertisementId = 1,
                PlayerPositionId = 12,
                Age = 37,
                Height = 167,
                PlayerFootId = 1,
                Salary = 234,
                AdditionalInformation = "no info",
                PlayerId = "leomessi"
            };
            var response1 = await _client.PostAsJsonAsync("/api/player-offers", newAd);
            var offerToAccept = await _fixture.DbContext.PlayerOffers.FirstOrDefaultAsync(co => co.Salary == 234);

            // Act
            var response2 = await _client.PutAsJsonAsync($"/api/player-offers/accept/{offerToAccept.Id}", offerToAccept);

            // Assert
            response2.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response2.StatusCode);

            var clubOffer = await _fixture.DbContext.PlayerOffers.FirstOrDefaultAsync(co => co.Salary == 234);
            _fixture.DbContext.PlayerOffers.Remove(clubOffer);
            await _fixture.DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task AcceptPlayerOffer_ReturnsNotFound_WhenPlayerOfferDoesNotExist()
        {
            // Arrange
            var clubAdvertisementId = 2;
            var advertisementToUpdate = await _fixture.DbContext.PlayerOffers.FirstAsync();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/player-offers/accept/{clubAdvertisementId}", advertisementToUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task RejectPlayerOffer_ReturnsNoContent_WhenPlayerOfferExists()
        {
            // Arrange
            var newAd = new PlayerOfferCreateDTO
            {
                ClubAdvertisementId = 1,
                PlayerPositionId = 12,
                Age = 37,
                Height = 167,
                PlayerFootId = 1,
                Salary = 235,
                AdditionalInformation = "no info",
                PlayerId = "leomessi"
            };
            var response1 = await _client.PostAsJsonAsync("/api/player-offers", newAd);
            var offerToAccept = await _fixture.DbContext.PlayerOffers.FirstOrDefaultAsync(co => co.Salary == 235);

            // Act
            var response2 = await _client.PutAsJsonAsync($"/api/player-offers/reject/{offerToAccept.Id}", offerToAccept);

            // Assert
            response2.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response2.StatusCode);

            var clubOffer = await _fixture.DbContext.PlayerOffers.FirstOrDefaultAsync(co => co.Salary == 235);
            _fixture.DbContext.PlayerOffers.Remove(clubOffer);
            await _fixture.DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task RejectPlayerOffer_ReturnsNotFound_WhenPlayerOfferDoesNotExist()
        {
            // Arrange
            var clubAdvertisementId = 2;
            var advertisementToUpdate = await _fixture.DbContext.PlayerOffers.FirstAsync();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/player-offers/reject/{clubAdvertisementId}", advertisementToUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetPlayerOfferStatusId_ReturnsOk_WhenPlayerOfferExists()
        {
            // Arrange
            var clubOfferId = 2;
            var playerId = "leomessi";

            // Act
            var response = await _client.GetAsync($"/api/player-offers/status/{clubOfferId}/{playerId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var offerStatusId = await response.Content.ReadFromJsonAsync<int>();
            Assert.Equal(2, offerStatusId);
        }

        [Fact]
        public async Task ExportPlayerOffersToCsv_ReturnsFileResult()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/player-offers/export");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/csv", response.Content.Headers.ContentType.MediaType);
            Assert.Equal("player-offers.csv", response.Content.Headers.ContentDisposition.FileName);
        }
    }
}