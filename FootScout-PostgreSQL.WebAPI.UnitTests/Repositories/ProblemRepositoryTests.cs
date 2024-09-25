using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using FootScout_PostgreSQL.WebAPI.UnitTests.TestManager;

namespace FootScout_PostgreSQL.WebAPI.UnitTests.Repositories
{
    public class ProblemRepositoryTests : TestBase
    {
        [Fact]
        public async Task GetProblem_ReturnsCorrectProblem()
        {
            // Arrange
            var options = GetDbContextOptions("GetProblem_ReturnsCorrectProblem");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedProblemTestBase(dbContext);
                var _problemRepository = new ProblemRepository(dbContext);

                // Act
                var result = await _problemRepository.GetProblem(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal("Problem 1", result.Title);
            }
        }

        [Fact]
        public async Task GetAllProblems_ReturnsAllProblemsOrderedByCreationDate()
        {
            // Arrange
            var options = GetDbContextOptions("GetAllProblems_ReturnsAllProblemsOrderedByCreationDate");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedProblemTestBase(dbContext);
                var _problemRepository = new ProblemRepository(dbContext);

                // Act
                var result = await _problemRepository.GetAllProblems();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
                Assert.True(result.First().CreationDate >= result.Last().CreationDate);
            }
        }

        [Fact]
        public async Task GetSolvedProblems_ReturnsOnlySolvedProblems()
        {
            // Arrange
            var options = GetDbContextOptions("GetSolvedProblems_ReturnsOnlySolvedProblems");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedProblemTestBase(dbContext);
                var _problemRepository = new ProblemRepository(dbContext);

                // Act
                var result = await _problemRepository.GetSolvedProblems();

                // Assert
                Assert.NotNull(result);
                Assert.All(result, problem => Assert.True(problem.IsSolved));
            }
        }

        [Fact]
        public async Task GetSolvedProblemCount_ReturnsCorrectCount()
        {
            // Arrange
            var options = GetDbContextOptions("GetSolvedProblemCount_ReturnsCorrectCount");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedProblemTestBase(dbContext);
                var _problemRepository = new ProblemRepository(dbContext);

                // Act
                var count = await _problemRepository.GetSolvedProblemCount();

                // Assert
                Assert.Equal(1, count);
            }
        }

        [Fact]
        public async Task GetUnsolvedProblems_ReturnsOnlyUnsolvedProblems()
        {
            // Arrange
            var options = GetDbContextOptions("GetUnsolvedProblems_ReturnsOnlyUnsolvedProblems");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedProblemTestBase(dbContext);
                var _problemRepository = new ProblemRepository(dbContext);

                // Act
                var result = await _problemRepository.GetUnsolvedProblems();

                // Assert
                Assert.NotNull(result);
                Assert.All(result, problem => Assert.False(problem.IsSolved));
            }
        }

        [Fact]
        public async Task GetUnsolvedProblemCount_ReturnsCorrectCount()
        {
            // Arrange
            var options = GetDbContextOptions("GetUnsolvedProblemCount_ReturnsCorrectCount");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedProblemTestBase(dbContext);
                var _problemRepository = new ProblemRepository(dbContext);

                // Act
                var count = await _problemRepository.GetUnsolvedProblemCount();

                // Assert
                Assert.Equal(1, count);
            }
        }

        [Fact]
        public async Task CreateProblem_AddsProblemToDatabase()
        {
            // Arrange
            var options = GetDbContextOptions("CreateProblem_AddsProblemToDatabase");
            var newProblem = new Problem
            {
                Id = 1,
                Title = "Problem 1",
                Description = "Desc 1",
                CreationDate = DateTime.UtcNow,
                IsSolved = false,
                RequesterId = "leomessi"
            };

            using (var dbContext = new AppDbContext(options))
            {
                var _problemRepository = new ProblemRepository(dbContext);

                // Act
                await _problemRepository.CreateProblem(newProblem);

                // Assert
                var result = await dbContext.Problems.FindAsync(1);
                Assert.NotNull(result);
                Assert.Equal("Problem 1", result.Title);
                Assert.False(result.IsSolved);
            }
        }

        [Fact]
        public async Task CheckProblemSolved_UpdatesProblemStatus()
        {
            // Arrange
            var options = GetDbContextOptions("CheckProblemSolved_UpdatesProblemStatus");
            var problem = new Problem
            {
                Id = 1,
                Title = "Problem 1",
                Description = "Desc 1",
                CreationDate = DateTime.UtcNow,
                IsSolved = true,
                RequesterId = "leomessi"
            };

            using (var dbContext = new AppDbContext(options))
            {
                var _problemRepository = new ProblemRepository(dbContext);
                await dbContext.Problems.AddAsync(problem);
                await dbContext.SaveChangesAsync();

                // Act
                problem.IsSolved = true;
                await _problemRepository.CheckProblemSolved(problem);

                // Assert
                var updatedProblem = await dbContext.Problems.FindAsync(1);
                Assert.NotNull(updatedProblem);
                Assert.True(updatedProblem.IsSolved);
            }
        }

        [Fact]
        public async Task ExportProblemsToCsv_ReturnsCsvStream()
        {
            // Arrange
            var options = GetDbContextOptions("ExportProblemsToCsv_ReturnsCsvStream");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedProblemTestBase(dbContext);
                var _problemRepository = new ProblemRepository(dbContext);

                // Act
                var csvStream = await _problemRepository.ExportProblemsToCsv();
                csvStream.Position = 0;

                using (var reader = new StreamReader(csvStream))
                {
                    var csvContent = await reader.ReadToEndAsync();

                    // Assert
                    Assert.NotEmpty(csvContent);
                    Assert.Contains("Problem Id,Is Solved,Requester E-mail,Requester First Name,Requester Last Name,Title,Description,Creation Date", csvContent);
                    Assert.Contains("1,True,lm10@gmail.com", csvContent);
                }
            }
        }
    }
}