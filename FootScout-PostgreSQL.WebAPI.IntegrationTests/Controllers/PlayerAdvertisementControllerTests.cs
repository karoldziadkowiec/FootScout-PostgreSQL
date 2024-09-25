using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using FootScout_PostgreSQL.WebAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Controllers
{
    public class PlayerAdvertisementControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public PlayerAdvertisementControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
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
        public async Task GetPlayerAdvertisement_ReturnsOk_WhenPlayerAdvertisementExists()
        {
            // Arrange
            var playerAdvertisementId = 1;

            // Act
            var response = await _client.GetAsync($"/api/player-advertisements/{playerAdvertisementId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var playerAdvertisement = await response.Content.ReadFromJsonAsync<PlayerAdvertisement>();
            Assert.Equal(playerAdvertisementId, playerAdvertisement.Id);
        }

        [Fact]
        public async Task GetPlayerAdvertisement_ReturnsNotFound_WhenPlayerAdvertisementDoesNotExist()
        {
            // Arrange
            var playerAdvertisementId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/player-advertisements/{playerAdvertisementId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllPlayerAdvertisements_ReturnsOk_WhenPlayerAdvertisementsExists()
        {
            // Arrange & Act
            var response = await _client.GetAsync($"/api/player-advertisements");

            // Assert
            response.EnsureSuccessStatusCode();
            var playerAdvertisements = await response.Content.ReadFromJsonAsync<IEnumerable<PlayerAdvertisement>>();
            Assert.NotEmpty(playerAdvertisements);
        }

        [Fact]
        public async Task GetActivePlayerAdvertisements_ReturnsOk_WhenActivePlayerAdvertisementsExist()
        {
            // Act
            var response = await _client.GetAsync("/api/player-advertisements/active");

            // Assert
            response.EnsureSuccessStatusCode();
            var activePlayerAdvertisements = await response.Content.ReadFromJsonAsync<IEnumerable<PlayerAdvertisement>>();
            Assert.NotEmpty(activePlayerAdvertisements);
        }

        [Fact]
        public async Task GetActivePlayerAdvertisementsCount_ReturnsOk_WithCorrectCount()
        {
            // Arrange
            var expectedCount = 2;

            // Act
            var response = await _client.GetAsync("/api/player-advertisements/active/count");

            // Assert
            response.EnsureSuccessStatusCode();
            var countString = await response.Content.ReadAsStringAsync();
            int actualCount = int.Parse(countString);
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetInactivePlayerAdvertisements_ReturnsOk_WhenInactivePlayerAdvertisementsExist()
        {
            // Act
            var response = await _client.GetAsync("/api/player-advertisements/inactive");

            // Assert
            response.EnsureSuccessStatusCode();
            var inactivePlayerAdvertisements = await response.Content.ReadFromJsonAsync<IEnumerable<PlayerAdvertisement>>();
            Assert.Empty(inactivePlayerAdvertisements);
        }

        [Fact]
        public async Task CreatePlayerAdvertisement_ReturnsOk_WhenDataIsValid()
        {
            // Arrange
            var adDto = new PlayerAdvertisementCreateDTO
            {
                PlayerPositionId = 12,
                League = "Serie A",
                Region = "Italy",
                Age = 37,
                Height = 167,
                PlayerFootId = 1,
                SalaryRangeDTO = new SalaryRangeDTO { Min = 251, Max = 301 },
                PlayerId = "leomessi"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/player-advertisements", adDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PlayerAdvertisement>();
            Assert.NotNull(result);
            Assert.Equal(adDto.PlayerPositionId, result.PlayerPositionId);
            Assert.Equal(adDto.League, result.League);
            Assert.Equal(adDto.Age, result.Age);
            Assert.Equal(adDto.Height, result.Height);
            Assert.Equal(adDto.PlayerId, result.PlayerId);

            var playerAdvertisement = await _fixture.DbContext.PlayerAdvertisements.FirstOrDefaultAsync(pa => pa.PlayerPositionId == adDto.PlayerPositionId && pa.League == adDto.League && pa.Age == adDto.Age && pa.Region == adDto.Region && pa.PlayerId == adDto.PlayerId);
            _fixture.DbContext.PlayerAdvertisements.Remove(playerAdvertisement);
            await _fixture.DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task CreatePlayerAdvertisement_ReturnsBadRequest_WhenDtoIsInvalid()
        {
            // Arrange
            PlayerAdvertisementCreateDTO invalidDto = null;

            // Act
            var response = await _client.PostAsJsonAsync("/api/player-advertisements", invalidDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorMessage = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task UpdatePlayerAdvertisement_ReturnsNoContent_WhenPlayerAdvertisementExists()
        {
            // Arrange
            var advertisementToUpdate = await _fixture.DbContext.PlayerAdvertisements.FirstAsync();
            advertisementToUpdate.Height = 168;

            // Act
            var response = await _client.PutAsJsonAsync($"/api/player-advertisements/{advertisementToUpdate.Id}", advertisementToUpdate);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePlayerAdvertisement_ReturnsNotFound_WhenPlayerAdvertisementDoesNotExist()
        {
            // Arrange
            var playerAdvertisementId = 2;
            var advertisementToUpdate = await _fixture.DbContext.PlayerAdvertisements.FirstAsync();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/player-advertisements/{playerAdvertisementId}", advertisementToUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteFromFavorites_DeletePlayerAdFromFavorites()
        {
            // Arrange
            var newAd = new PlayerAdvertisementCreateDTO
            {
                PlayerPositionId = 12,
                League = "Serie A",
                Region = "Italy",
                Age = 37,
                Height = 167,
                PlayerFootId = 1,
                SalaryRangeDTO = new SalaryRangeDTO { Min = 252, Max = 302 },
                PlayerId = "leomessi"
            };
            var response = await _client.PostAsJsonAsync("/api/player-advertisements", newAd);
            var playerAdvertisement = await _fixture.DbContext.PlayerAdvertisements.FirstOrDefaultAsync(pa => pa.PlayerPositionId == newAd.PlayerPositionId && pa.League == newAd.League && pa.Age == newAd.Age && pa.Region == newAd.Region && pa.PlayerId == newAd.PlayerId);

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/player-advertisements/{playerAdvertisement.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task ExportPlayerAdvertisementsToCsv_ReturnsFileResult()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/player-advertisements/export");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/csv", response.Content.Headers.ContentType.MediaType);
            Assert.Equal("player-advertisements.csv", response.Content.Headers.ContentDisposition.FileName);
        }
    }
}