using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FootScout_PostgreSQL.WebAPI.DbManager;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FootScout_PostgreSQL.WebAPI.Entities;
using System.Net;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Controllers
{
    public class PlayerPositionControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public PlayerPositionControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
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
        public async Task GetPlayerPositions_ReturnsOk_WhenPlayerPositionsExists()
        {
            // Arrange & Act
            var response = await _client.GetAsync($"/api/player-positions");

            // Assert
            response.EnsureSuccessStatusCode();
            var playerPositions = await response.Content.ReadFromJsonAsync<IEnumerable<PlayerPosition>>();
            Assert.NotEmpty(playerPositions);
        }

        [Fact]
        public async Task GetPlayerPositionCount_ReturnsOk_WithCorrectCount()
        {
            // Arrange
            var expectedCount = 15;

            // Act
            var response = await _client.GetAsync("/api/player-positions/count");

            // Assert
            response.EnsureSuccessStatusCode();

            var countString = await response.Content.ReadAsStringAsync();
            int actualCount = int.Parse(countString);

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetPlayerPositionName_ReturnsOk_WhenPositionExists()
        {
            // Arrange
            var positionId = 1;

            // Act
            var response = await _client.GetAsync($"/api/player-positions/{positionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var positionName = await response.Content.ReadAsStringAsync();
            Assert.Equal("Goalkeeper", positionName);
        }

        [Fact]
        public async Task GetPlayerPositionName_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            // Arrange
            var positionId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/player-positions/{positionId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CheckPlayerPositionExists_ReturnsTrue_WhenPositionExists()
        {
            // Arrange
            var positionName = "Goalkeeper";

            // Act
            var response = await _client.GetAsync($"/api/player-positions/check/name/{positionName}");

            // Assert
            response.EnsureSuccessStatusCode();
            var isExists = await response.Content.ReadAsStringAsync();
            Assert.Equal("true", isExists);
        }

        [Fact]
        public async Task CheckPlayerPositionExists_ReturnsFalse_WhenPositionDoesNotExist()
        {
            // Arrange
            var positionName = "UnknownPosition";

            // Act
            var response = await _client.GetAsync($"/api/player-positions/check/name/{positionName}");

            // Assert
            response.EnsureSuccessStatusCode();
            var isExists = await response.Content.ReadAsStringAsync();
            Assert.Equal("false", isExists);
        }

        [Fact]
        public async Task CreatePlayerPosition_ReturnsOk_WhenPositionIsValid()
        {
            // Arrange
            var playerPosition = new PlayerPosition { PositionName = "NewPosition" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/player-positions", playerPosition);

            // Assert
            response.EnsureSuccessStatusCode();
            var createdPosition = await response.Content.ReadFromJsonAsync<PlayerPosition>();
            Assert.Equal(playerPosition.PositionName, createdPosition.PositionName);

            var playerPosition2 = await _fixture.DbContext.PlayerPositions.FirstOrDefaultAsync(pp => pp.PositionName == "NewPosition");
            _fixture.DbContext.PlayerPositions.Remove(playerPosition2);
            await _fixture.DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task CreatePlayerPosition_ReturnsBadRequest_WhenPositionIsNull()
        {
            // Arrange
            PlayerPosition playerPosition = null;

            // Act
            var response = await _client.PostAsJsonAsync("/api/player-positions", playerPosition);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
