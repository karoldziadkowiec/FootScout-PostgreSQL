using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using FootScout_PostgreSQL.WebAPI.UnitTests.TestManager;

namespace FootScout_PostgreSQL.WebAPI.UnitTests.Repositories
{
    public class SalaryRangeRepositoryTests : TestBase
    {
        [Fact]
        public async Task CreateSalaryRange_AddsNewSalaryRange()
        {
            // Arrange
            var options = GetDbContextOptions("CreateSalaryRange_AddsNewSalaryRange");

            var newSalaryRange = new SalaryRange
            {
                Id = 1,
                Min = 80.0,
                Max = 160.0
            };

            using (var dbContext = new AppDbContext(options))
            {
                var _salaryRangeRepository = new SalaryRangeRepository(dbContext);

                // Act
                await _salaryRangeRepository.CreateSalaryRange(newSalaryRange);

                // Assert
                var result = await dbContext.SalaryRanges.FindAsync(1);
                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal(80.0, result.Min);
                Assert.Equal(160.0, result.Max);
            }
        }
    }
}