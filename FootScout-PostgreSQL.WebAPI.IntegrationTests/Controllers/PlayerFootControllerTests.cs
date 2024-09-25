using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Controllers
{
    public class PlayerFootControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public PlayerFootControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
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
        public async Task GetPlayerFeet_ReturnsOk_WhenPlayerFeetExists()
        {
            // Arrange & Act
            var response = await _client.GetAsync($"/api/player-feet");

            // Assert
            response.EnsureSuccessStatusCode();
            var playerFeet = await response.Content.ReadFromJsonAsync<IEnumerable<PlayerFoot>>();
            Assert.NotEmpty(playerFeet);
        }

        [Fact]
        public async Task GetPlayerFootName_ReturnsOk_WhenFootExists()
        {
            // Arrange
            var footId = 1;

            // Act
            var response = await _client.GetAsync($"/api/player-feet/{footId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var footName = await response.Content.ReadAsStringAsync();
            Assert.Equal("Left", footName);
        }
    }
}