using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using FootScout_PostgreSQL.WebAPI.UnitTests.TestManager;

namespace FootScout_PostgreSQL.WebAPI.UnitTests.Repositories
{
    public class PlayerFootRepositoryTests : TestBase
    {
        [Fact]
        public async Task GetPlayerFoot_ReturnsCorrectPlayerFoot()
        {
            // Arrange
            var options = GetDbContextOptions("GetPlayerFoot_ReturnsCorrectPlayerFoot");

            using (var dbContext = new AppDbContext(options))
            {
                await SeedPlayerFootTestBase(dbContext);
                var _playerFootRepository = new PlayerFootRepository(dbContext);

                // Act
                var result = await _playerFootRepository.GetPlayerFoot(1);

                // Assert
                Assert.Equal("Left", result.FootName);
            }
        }

        [Fact]
        public async Task GetPlayerFeet_ReturnsAllPlayerFeet()
        {
            // Arrange
            var options = GetDbContextOptions("GetPlayerFeet_ReturnsAllPlayerFeet");

            using (var dbContext = new AppDbContext(options))
            {
                await SeedPlayerFootTestBase(dbContext);
                var _playerFootRepository = new PlayerFootRepository(dbContext);

                // Act
                var result = await _playerFootRepository.GetPlayerFeet();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(3, result.Count());
            }
        }

        [Fact]
        public async Task GetPlayerFootName_ReturnsPlayerFootName()
        {
            // Arrange
            var options = GetDbContextOptions("GetPlayerFootName_ReturnsPlayerFootName");

            using (var dbContext = new AppDbContext(options))
            {
                await SeedPlayerFootTestBase(dbContext);
                var _playerFootRepository = new PlayerFootRepository(dbContext);

                // Act
                var result = await _playerFootRepository.GetPlayerFootName(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Left", result);
            }
        }
    }
}