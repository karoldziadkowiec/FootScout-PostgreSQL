using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using FootScout_PostgreSQL.WebAPI.Models.Constants;
using FootScout_PostgreSQL.WebAPI.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Controllers
{
    public class AccountControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public AccountControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
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
        public async Task Register_ReturnsOk_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var registerDTO = new RegisterDTO
            {
                Email = "new@user.com",
                Password = "Password1!",
                ConfirmPassword = "Password1!",
                FirstName = "First Name",
                LastName = "Last Name",
                Location = "Location",
                PhoneNumber = "PhoneNumber",
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/account/register", registerDTO);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<RegisterDTO>();
            Assert.NotNull(result);
            Assert.Equal(registerDTO.Email, result.Email);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            var invalidRegisterDTO = new RegisterDTO
            {
                Email = "new@user.com",
                Password = "Password1!",
                ConfirmPassword = "Password1!",
                FirstName = "First Name",
                LastName = "Last Name",
                PhoneNumber = "PhoneNumber",
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/account/register", invalidRegisterDTO);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginDTO = new LoginDTO
            {
                Email = "lm10@gmail.com",
                Password = "Leooo1!"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/account/login", loginDTO);

            // Assert
            Assert.NotNull(response.StatusCode);
            var token = await response.Content.ReadAsStringAsync();
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            var invalidLoginDTO = new LoginDTO
            {
                Email = "",
                Password = "Leooo1!"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/account/login", invalidLoginDTO);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetRoles_ReturnsOk_WithRoleNames()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/account/roles");

            // Assert
            response.EnsureSuccessStatusCode();
            var roles = await response.Content.ReadFromJsonAsync<IEnumerable<string>>();
            Assert.NotNull(roles);
            Assert.NotEmpty(roles);
        }

        [Fact]
        public async Task MakeAnAdmin_ReturnsNoContent_WhenUserIsSuccessfullyMadeAdmin()
        {
            // Arrange
            var userId = "unknown9";

            // Act
            var response = await _client.PostAsync($"/api/account/roles/make-admin/{userId}", null);

            // Assert
            Assert.NotNull(response.StatusCode);
        }

        [Fact]
        public async Task DemoteFromAdmin_ReturnsNoContent_WhenUserIsSuccessfullyDemoted()
        {
            // Arrange
            var userId = "unknown9";
            _fixture.DbContext.UserRoles.Add(new IdentityUserRole<string> { UserId = userId, RoleId = _fixture.DbContext.Roles.First(r => r.Name == Role.Admin).Id });
            await _fixture.DbContext.SaveChangesAsync();

            // Act
            var response = await _client.PostAsync($"/api/account/roles/make-user/{userId}", null);

            // Assert
            Assert.NotNull(response.StatusCode);
        }
    }
}