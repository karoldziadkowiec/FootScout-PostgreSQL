using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Repositories
{
    public  class ProblemRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly AppDbContext _dbContext;
        private readonly ProblemRepository _problemRepository;

        public ProblemRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _problemRepository = new ProblemRepository(_dbContext);
        }

        [Fact]
        public async Task GetProblem_ReturnsCorrectProblem()
        {
            // Arrange
            var problemId = 1;

            // Act
            var result = await _problemRepository.GetProblem(problemId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(problemId, result.Id);
            Assert.Equal("Problem 1", result.Title);
        }

        [Fact]
        public async Task GetAllProblems_ReturnsAllProblemsOrderedByCreationDate()
        {
            // Arrange & Act
            var result = await _problemRepository.GetAllProblems();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.True(result.First().CreationDate >= result.Last().CreationDate);
        }

        [Fact]
        public async Task GetSolvedProblems_ReturnsOnlySolvedProblems()
        {
            // Arrange & Act
            var result = await _problemRepository.GetSolvedProblems();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, problem => Assert.True(problem.IsSolved));
        }

        [Fact]
        public async Task GetSolvedProblemCount_ReturnsCorrectCount()
        {
            // Arrange & Act
            var count = await _problemRepository.GetSolvedProblemCount();

            // Assert
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task GetUnsolvedProblems_ReturnsOnlyUnsolvedProblems()
        {
            // Arrange & Act
            var result = await _problemRepository.GetUnsolvedProblems();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, problem => Assert.False(problem.IsSolved));
        }

        [Fact]
        public async Task GetUnsolvedProblemCount_ReturnsCorrectCount()
        {
            // Arrange & Act
            var count = await _problemRepository.GetUnsolvedProblemCount();

            // Assert
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CreateProblem_AddsProblemToDatabase()
        {
            // Arrange
            var newProblem = new Problem
            {
                Title = "Problem 1",
                Description = "Desc 1",
                CreationDate = DateTime.UtcNow,
                IsSolved = false,
                RequesterId = "leomessi"
            };

            // Act
            await _problemRepository.CreateProblem(newProblem);

            // Assert
            var result = await _dbContext.Problems.FindAsync(3);
            Assert.NotNull(result);
            Assert.Equal("Problem 1", result.Title);
            Assert.False(result.IsSolved);

            _dbContext.Problems.Remove(result);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task CheckProblemSolved_UpdatesProblemStatus()
        {
            // Arrange
            var problemId = 1;

            // Act
            var problem = await _dbContext.Problems.FindAsync(problemId);
            problem.IsSolved = true;
            await _problemRepository.CheckProblemSolved(problem);

            // Assert
            var updatedProblem = await _dbContext.Problems.FindAsync(problemId);
            Assert.NotNull(updatedProblem);
            Assert.True(updatedProblem.IsSolved);
        }

        [Fact]
        public async Task ExportProblemsToCsv_ReturnsCsvStream()
        {
            // Arrange & Act
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
