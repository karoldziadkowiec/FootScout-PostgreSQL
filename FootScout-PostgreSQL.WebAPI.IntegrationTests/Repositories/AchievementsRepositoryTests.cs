using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Repositories
{
    public class AchievementsRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly AppDbContext _dbContext;
        private readonly AchievementsRepository _achievementsRepository;

        public AchievementsRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _achievementsRepository = new AchievementsRepository(_dbContext);
        }

        [Fact]
        public async Task CreateAchievements_AddsNewAchievements()
        {
            // Arrange
            var newAchievements = new Achievements
            {
                NumberOfMatches = 80,
                Goals = 60,
                Assists = 45,
                AdditionalAchievements = "no"
            };

            // Act
            await _achievementsRepository.CreateAchievements(newAchievements);

            // Assert
            var result = await _dbContext.Achievements.FindAsync(3);
            Assert.NotNull(result);
            Assert.Equal(80, result.NumberOfMatches);
            Assert.Equal(60, result.Goals);
            Assert.Equal(45, result.Assists);

            _dbContext.Achievements.Remove(result);
            await _dbContext.SaveChangesAsync();
        }
    }
}