using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using Microsoft.EntityFrameworkCore;
using FootScout_PostgreSQL.WebAPI.DbManager;
using Microsoft.Extensions.DependencyInjection;
using FootScout_PostgreSQL.WebAPI.Models.DTOs;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Controllers
{
    public class ProblemControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public ProblemControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
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
        public async Task GetProblem_ReturnsOk_WhenProblemExists()
        {
            // Arrange
            var problemId = 1;

            // Act
            var response = await _client.GetAsync($"/api/problems/{problemId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var problem = await response.Content.ReadFromJsonAsync<OfferStatus>();
            Assert.Equal(problemId, problem.Id);
        }

        [Fact]
        public async Task GetProblem_ReturnsNotFound_WhenProblemDoesNotExist()
        {
            // Arrange
            var problemId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/problems/{problemId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllProblems_ReturnsOk_WhenProblemsExists()
        {
            // Arrange & Act
            var response = await _client.GetAsync($"/api/problems");

            // Assert
            response.EnsureSuccessStatusCode();
            var problems = await response.Content.ReadFromJsonAsync<IEnumerable<Problem>>();
            Assert.NotEmpty(problems);
        }

        [Fact]
        public async Task GetSolvedProblems_ReturnsOk_WhenSolvedProblemsExist()
        {
            // Act
            var response = await _client.GetAsync("/api/problems/solved");

            // Assert
            response.EnsureSuccessStatusCode();
            var solvedProblems = await response.Content.ReadFromJsonAsync<IEnumerable<Problem>>();
            Assert.NotEmpty(solvedProblems);
        }

        [Fact]
        public async Task GetSolvedProblemCount_ReturnsOk_WithCorrectCount()
        {
            // Arrange
            var expectedCount = 1;

            // Act
            var response = await _client.GetAsync("/api/problems/solved/count");

            // Assert
            response.EnsureSuccessStatusCode();
            var countString = await response.Content.ReadAsStringAsync();
            int actualCount = int.Parse(countString);
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetUnsolvedProblems_ReturnsOk_WhenUnsolvedProblemsExist()
        {
            // Act
            var response = await _client.GetAsync("/api/problems/unsolved");

            // Assert
            response.EnsureSuccessStatusCode();
            var unsolvedProblems = await response.Content.ReadFromJsonAsync<IEnumerable<Problem>>();
            Assert.NotEmpty(unsolvedProblems);
        }

        [Fact]
        public async Task GetUnsolvedProblemCount_ReturnsOk_WithCorrectCount()
        {
            // Arrange
            var expectedCount = 1;

            // Act
            var response = await _client.GetAsync("/api/problems/unsolved/count");

            // Assert
            response.EnsureSuccessStatusCode();
            var countString = await response.Content.ReadAsStringAsync();
            int actualCount = int.Parse(countString);
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task CreateProblem_ReturnsOk_WhenProblemIsCreatedSuccessfully()
        {
            // Arrange
            var dto = new ProblemCreateDTO
            {
                Title = "Problem",
                Description = "Problem DESC",
                RequesterId = "leomessi"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/problems", dto);

            // Assert
            response.EnsureSuccessStatusCode();
            var createdProblem = await response.Content.ReadFromJsonAsync<Problem>();
            Assert.Equal(dto.Title, createdProblem.Title);

            var problem = await _fixture.DbContext.Problems.FirstOrDefaultAsync(ch => ch.Title == "Problem");
            _fixture.DbContext.Problems.Remove(problem);
            await _fixture.DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task CreateProblem_InvalidDto_ReturnsBadRequest()
        {
            // Arrange
            ProblemCreateDTO dto = null;

            // Act
            var response = await _client.PostAsJsonAsync("/api/problems", dto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CheckProblemSolved_ReturnsNoContent_WhenProblemIsUpdatedSuccessfully()
        {
            // Arrange
            var dto = new ProblemCreateDTO
            {
                Title = "Problem 3",
                Description = "Problem DESC",
                RequesterId = "leomessi"
            };
            var response1 = await _client.PostAsJsonAsync("/api/problems", dto);

            // Arrange
            var problem = await _fixture.DbContext.Problems.FirstOrDefaultAsync(p => p.Title == "Problem 3");

            // Act
            var response2 = await _client.PutAsJsonAsync($"/api/problems/{problem.Id}", problem);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response2.StatusCode);

            _fixture.DbContext.Problems.Remove(problem);
            await _fixture.DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task CheckProblemSolved_ReturnsBadRequest_WhenInvalidProblemId()
        {
            // Arrange
            var problemId = 1;
            var problem = await _fixture.DbContext.Problems.FirstOrDefaultAsync(p => p.Title == "Problem 2");

            // Act
            var response = await _client.PutAsJsonAsync($"/api/problems/{problemId}", problem);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ExportProblemsToCsv_ReturnsFile_WithCorrectContentType()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/problems/export");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/csv", response.Content.Headers.ContentType.MediaType);
            Assert.Equal("problems.csv", response.Content.Headers.ContentDisposition.FileName);
        }
    }
}