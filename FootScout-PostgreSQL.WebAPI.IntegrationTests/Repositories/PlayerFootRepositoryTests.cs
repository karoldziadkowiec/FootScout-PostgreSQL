using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using FootScout_PostgreSQL.WebAPI.Repositories.Interfaces;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Repositories
{
    public class PlayerFootRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly AppDbContext _dbContext;
        private readonly PlayerFootRepository _playerFootRepository;

        public PlayerFootRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _playerFootRepository = new PlayerFootRepository(_dbContext);
        }

        [Fact]
        public async Task GetPlayerFoot_ReturnsCorrectPlayerFoot()
        {
            // Arrange
            var playerFootId = 1;

            // Act
            var result = await _playerFootRepository.GetPlayerFoot(playerFootId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(playerFootId, result.Id);
            Assert.Equal("Left", result.FootName);
        }

        [Fact]
        public async Task GetPlayerFeet_ReturnsAllPlayerFeet()
        {
            // Arrange & Act
            var result = await _playerFootRepository.GetPlayerFeet();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetPlayerFootName_ReturnsPlayerFootName()
        {
            // Arrange
            var footId = 1;
            
            // Act
            var result = await _playerFootRepository.GetPlayerFootName(footId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Left", result);
        }
    }
}