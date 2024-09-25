using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Repositories
{
    public class SalaryRangeRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly AppDbContext _dbContext;
        private readonly SalaryRangeRepository _salaryRangeRepository;

        public SalaryRangeRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _salaryRangeRepository = new SalaryRangeRepository(_dbContext);
        }

        [Fact]
        public async Task CreateSalaryRange_AddsNewSalaryRange()
        {
            // Arrange
            var newSalaryRange = new SalaryRange
            {
                Min = 80.0,
                Max = 160.0
            };

            // Act
            await _salaryRangeRepository.CreateSalaryRange(newSalaryRange);

            // Assert
            var result = await _dbContext.SalaryRanges.FindAsync(5);
            Assert.NotNull(result);
            Assert.Equal(5, result.Id);
            Assert.Equal(80.0, result.Min);
            Assert.Equal(160.0, result.Max);

            _dbContext.SalaryRanges.Remove(result);
            await _dbContext.SaveChangesAsync();
        }
    }
}