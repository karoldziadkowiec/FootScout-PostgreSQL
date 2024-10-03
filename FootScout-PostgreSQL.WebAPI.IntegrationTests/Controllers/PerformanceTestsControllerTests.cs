using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Controllers
{
    public class PerformanceTestsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public PerformanceTestsControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
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

            var adminTokenJWT = _fixture.AdminTokenJWT;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminTokenJWT);
        }

        [Fact]
        public async Task SeedComponents_ReturnsNoContent_WhenValidTestCounterNumber()
        {
            // Arrange
            var testCounter = 10;

            // Act
            var response = await _client.PostAsync($"/api/performance-tests/seed/{testCounter}", null);

            // Assert
            Assert.NotNull(response.StatusCode);

            var usersCount = await _fixture.DbContext.Users.CountAsync();
            Assert.True(usersCount >= testCounter);

            var userRolesCount = await _fixture.DbContext.UserRoles.CountAsync();
            Assert.True(userRolesCount >= testCounter);

            // Cleaning
            var clearResponse2 = await _client.DeleteAsync("/api/performance-tests/clear");
            clearResponse2.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task SeedComponents_ReturnsBadRequest_WhenInvalidTestCounterNumber()
        {
            // Arrange
            var testCounter = -1;

            // Act
            var response = await _client.PostAsync($"/api/performance-tests/seed/{testCounter}", null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ClearDatabaseOfSeededComponents_ReturnsNoContent_WhenProccessSucceded()
        {
            // Arrange
            var expectedValue = 0;

            // Act
            var response = await _client.DeleteAsync($"/api/performance-tests/clear");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            response.EnsureSuccessStatusCode();

            var chatsCount = await _fixture.DbContext.Chats.CountAsync();
            Assert.Equal(expectedValue, chatsCount);

            var messagesCount = await _fixture.DbContext.Messages.CountAsync();
            Assert.Equal(expectedValue, messagesCount);

            var playerAdvertisementsCount = await _fixture.DbContext.PlayerAdvertisements.CountAsync();
            Assert.Equal(expectedValue, playerAdvertisementsCount);

            var favoritePlayerAdvertisementsCount = await _fixture.DbContext.FavoritePlayerAdvertisements.CountAsync();
            Assert.Equal(expectedValue, favoritePlayerAdvertisementsCount);

            var clubOffersCount = await _fixture.DbContext.ClubOffers.CountAsync();
            Assert.Equal(expectedValue, clubOffersCount);

            var salaryRangesCount = await _fixture.DbContext.SalaryRanges.CountAsync();
            Assert.Equal(expectedValue, salaryRangesCount);
        }
    }
}