using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using Microsoft.EntityFrameworkCore;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Repositories
{
    public class PlayerAdvertisementRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly AppDbContext _dbContext;
        private readonly PlayerAdvertisementRepository _playerAdvertisementRepository;

        public PlayerAdvertisementRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _playerAdvertisementRepository = new PlayerAdvertisementRepository(_dbContext);
        }

        [Fact]
        public async Task GetPlayerAdvertisement_ReturnsCorrectPlayerAdvertisement()
        {
            // Arrange & Act
            var result = await _playerAdvertisementRepository.GetPlayerAdvertisement(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Premier League", result.League);
            Assert.Equal("England", result.Region);
        }

        [Fact]
        public async Task GetAllPlayerAdvertisements_ReturnsAllPlayerAdvertisementsOrderedDESCByEndDate()
        {
            // Arrange & Act
            var result = await _playerAdvertisementRepository.GetAllPlayerAdvertisements();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.True(result.First().CreationDate >= result.Last().CreationDate);
        }

        [Fact]
        public async Task GetActivePlayerAdvertisements_ReturnsActiveAdvertisements()
        {
            // Arrange & Act
            var result = await _playerAdvertisementRepository.GetActivePlayerAdvertisements();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, ad => Assert.True(ad.EndDate >= DateTime.UtcNow));
        }

        [Fact]
        public async Task GetActivePlayerAdvertisementCount_ReturnsCorrectCount()
        {
            // Arrange & Act
            var result = await _playerAdvertisementRepository.GetActivePlayerAdvertisementCount();

            // Assert
            var expectedCount = await _dbContext.PlayerAdvertisements
                .Where(pa => pa.EndDate >= DateTime.UtcNow)
                .CountAsync();

            Assert.Equal(expectedCount, result);
        }

        [Fact]
        public async Task GetInactivePlayerAdvertisements_ReturnsInactiveAdvertisements()
        {
            // Arrange & Act
            var result = await _playerAdvertisementRepository.GetInactivePlayerAdvertisements();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, ad => Assert.True(ad.EndDate < DateTime.UtcNow));
        }

        [Fact]
        public async Task CreatePlayerAdvertisement_SuccessfullyCreatesAdvertisement()
        {
            // Arrange
            _dbContext.SalaryRanges.Add(new SalaryRange { Min = 250, Max = 300 });
            await _dbContext.SaveChangesAsync();

            var salaryRange = await _dbContext.SalaryRanges
                .FirstOrDefaultAsync(sr => sr.Min == 250 && sr.Max == 300);

            var newAd = new PlayerAdvertisement
            {
                PlayerPositionId = 15,
                PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" },
                League = "Serie A",
                Region = "Italy",
                Age = 37,
                Height = 167,
                PlayerFootId = 1,
                PlayerFoot = new PlayerFoot { Id = 1, FootName = "Left" },
                SalaryRangeId = salaryRange.Id,
                CreationDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                PlayerId = "leomessi"
            };

            // Act
            await _playerAdvertisementRepository.CreatePlayerAdvertisement(newAd);

            var result = await _dbContext.PlayerAdvertisements.FindAsync(newAd.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newAd.League, result.League);
            Assert.Equal(newAd.Region, result.Region);
            Assert.True(result.CreationDate <= DateTime.UtcNow);
            Assert.True(result.EndDate > DateTime.UtcNow);

            _dbContext.PlayerAdvertisements.Remove(result);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdatePlayerAdvertisement_SuccessfullyUpdatesAdvertisement()
        {
            // Arrange
            var advertisementToUpdate = await _dbContext.PlayerAdvertisements.FirstAsync();
            advertisementToUpdate.Height = 168;

            // Act
            await _playerAdvertisementRepository.UpdatePlayerAdvertisement(advertisementToUpdate);

            // Assert
            var updatedAd = await _dbContext.PlayerAdvertisements.FindAsync(advertisementToUpdate.Id);
            Assert.NotNull(updatedAd);
            Assert.Equal(168, updatedAd.Height);
        }

        [Fact]
        public async Task DeletePlayerAdvertisement_RemovesPlayerAdvertisementAndRelatedEntities()
        {
            // Arrange
            _dbContext.SalaryRanges.Add(new SalaryRange { Min = 350, Max = 400 });
            await _dbContext.SaveChangesAsync();

            var salaryRange = await _dbContext.SalaryRanges
                .FirstOrDefaultAsync(sr => sr.Min == 350 && sr.Max == 400);

            _dbContext.PlayerAdvertisements.Add(new PlayerAdvertisement
            {
                PlayerPositionId = 15,
                PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" },
                League = "Bundesliga",
                Region = "Germany",
                Age = 37,
                Height = 167,
                PlayerFootId = 1,
                PlayerFoot = new PlayerFoot { Id = 1, FootName = "Left" },
                SalaryRangeId = salaryRange.Id,
                CreationDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                PlayerId = "leomessi"
            });
            await _dbContext.SaveChangesAsync();

            var advertisementToDelete = await _dbContext.PlayerAdvertisements
                .FirstOrDefaultAsync(pa => pa.League == "Bundesliga" && pa.Region == "Germany" && pa.PlayerId == "leomessi");

            // Act
            await _playerAdvertisementRepository.DeletePlayerAdvertisement(advertisementToDelete.Id);

            // Assert
            var deletedAdvertisement = await _dbContext.PlayerAdvertisements.FindAsync(advertisementToDelete.Id);
            Assert.Null(deletedAdvertisement);
        }

        [Fact]
        public async Task ExportPlayerAdvertisementsToCsv_ReturnsCsvFile()
        {
            // Arrange & Act
            var csvStream = await _playerAdvertisementRepository.ExportPlayerAdvertisementsToCsv();
            csvStream.Position = 0;

            using (var reader = new StreamReader(csvStream))
            {
                var csvContent = await reader.ReadToEndAsync();

                // Assert
                Assert.NotEmpty(csvContent);
                Assert.Contains("E-mail,First Name,Last Name,Position,League,Region,Age,Height,Foot,Min Salary,Max Salary,Creation Date,End Date", csvContent);
                Assert.Contains("lm10@gmail.com,Leo,Messi,Striker", csvContent);
            }
        }
    }
}