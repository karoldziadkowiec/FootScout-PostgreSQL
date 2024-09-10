using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using FootScout_PostgreSQL.WebAPI.UnitTests.TestManager;

namespace FootScout_PostgreSQL.WebAPI.UnitTests.Repositories
{
    public class PlayerPositionRepositoryTests : TestBase
    {
        [Fact]
        public async Task GetPlayerPositions_ReturnsAllPlayerPositions()
        {
            // Arrange
            var options = GetDbContextOptions("GetPlayerPositions_ReturnsAllPlayerPositions");

            using (var dbContext = new AppDbContext(options))
            {
                await SeedPlayerPositionTestBase(dbContext);
                var _playerPositionRepository = new PlayerPositionRepository(dbContext);

                // Act
                var result = await _playerPositionRepository.GetPlayerPositions();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(15, result.Count());
            }
        }

        [Fact]
        public async Task GetPlayerPositionCount_ReturnsCorrectCount()
        {
            // Arrange
            var options = GetDbContextOptions("GetPlayerPositionCount_ReturnsCorrectCount");

            using (var dbContext = new AppDbContext(options))
            {
                await SeedPlayerPositionTestBase(dbContext);
                var _playerPositionRepository = new PlayerPositionRepository(dbContext);

                // Act
                var result = await _playerPositionRepository.GetPlayerPositionCount();

                // Assert
                Assert.Equal(15, result);
            }
        }

        [Fact]
        public async Task GetPlayerPositionName_ReturnsCorrectName()
        {
            // Arrange
            var options = GetDbContextOptions("GetPlayerPositionName_ReturnsCorrectName");

            using (var dbContext = new AppDbContext(options))
            {
                await SeedPlayerPositionTestBase(dbContext);
                var _playerPositionRepository = new PlayerPositionRepository(dbContext);

                // Act
                var result = await _playerPositionRepository.GetPlayerPositionName(15);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Striker", result);
            }
        }

        [Fact]
        public async Task CheckPlayerPositionExists_ReturnsTrue_WhenPositionExists()
        {
            // Arrange
            var options = GetDbContextOptions("CheckPlayerPositionExists_ReturnsTrue_WhenPositionExists");

            using (var dbContext = new AppDbContext(options))
            {
                await SeedPlayerPositionTestBase(dbContext);
                var _playerPositionRepository = new PlayerPositionRepository(dbContext);

                // Act
                var result = await _playerPositionRepository.CheckPlayerPositionExists("Striker");

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public async Task CheckPlayerPositionExists_ReturnsFalse_WhenPositionDoesNotExist()
        {
            // Arrange
            var options = GetDbContextOptions("CheckPlayerPositionExists_ReturnsFalse_WhenPositionDoesNotExist");

            using (var dbContext = new AppDbContext(options))
            {
                await SeedPlayerPositionTestBase(dbContext);
                var _playerPositionRepository = new PlayerPositionRepository(dbContext);

                // Act
                var result = await _playerPositionRepository.CheckPlayerPositionExists("Trainer");

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async Task CreatePlayerPosition_AddsNewPosition_WhenPositionDoesNotExist()
        {
            // Arrange
            var options = GetDbContextOptions("CreatePlayerPosition_AddsNewPosition_WhenPositionDoesNotExist");
            var newPosition = new PlayerPosition { PositionName = "NewPosition" };

            using (var dbContext = new AppDbContext(options))
            {
                await SeedPlayerPositionTestBase(dbContext);
                var _playerPositionRepository = new PlayerPositionRepository(dbContext);

                // Act
                await _playerPositionRepository.CreatePlayerPosition(newPosition);

                // Assert
                var result = await dbContext.PlayerPositions.FindAsync(16);
                Assert.NotNull(result);
                Assert.Equal("NewPosition", result.PositionName);
            }
        }
    }
}