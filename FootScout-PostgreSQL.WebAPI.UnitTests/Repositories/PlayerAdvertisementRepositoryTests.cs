using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using FootScout_PostgreSQL.WebAPI.UnitTests.TestManager;
using Microsoft.EntityFrameworkCore;

namespace FootScout_PostgreSQL.WebAPI.UnitTests.Repositories
{
    public class PlayerAdvertisementRepositoryTests : TestBase
    {
        [Fact]
        public async Task GetPlayerAdvertisement_ReturnsCorrectPlayerAdvertisement()
        {
            // Arrange
            var options = GetDbContextOptions("GetPlayerAdvertisement_ReturnsCorrectPlayerAdvertisement");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedPlayerAdvertisementTestBase(dbContext);
                var _playerAdvertisementRepository = new PlayerAdvertisementRepository(dbContext);

                // Act
                var result = await _playerAdvertisementRepository.GetPlayerAdvertisement(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal("Premier League", result.League);
                Assert.Equal("England", result.Region);
            }
        }

        [Fact]
        public async Task GetAllPlayerAdvertisements_ReturnsAllPlayerAdvertisementsOrderedDESCByEndDate()
        {
            // Arrange
            var options = GetDbContextOptions("GetAllPlayerAdvertisements_ReturnsAllPlayerAdvertisementsOrderedDESCByEndDate");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedPlayerAdvertisementTestBase(dbContext);
                var _playerAdvertisementRepository = new PlayerAdvertisementRepository(dbContext);

                // Act
                var result = await _playerAdvertisementRepository.GetAllPlayerAdvertisements();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
                Assert.True(result.First().CreationDate >= result.Last().CreationDate);
            }
        }

        [Fact]
        public async Task GetActivePlayerAdvertisements_ReturnsActiveAdvertisements()
        {
            // Arrange
            var options = GetDbContextOptions("GetActivePlayerAdvertisements_ReturnsActiveAdvertisements");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedPlayerAdvertisementTestBase(dbContext);
                var _playerAdvertisementRepository = new PlayerAdvertisementRepository(dbContext);

                // Act
                var result = await _playerAdvertisementRepository.GetActivePlayerAdvertisements();

                // Assert
                Assert.NotNull(result);
                Assert.All(result, ad => Assert.True(ad.EndDate >= DateTime.UtcNow));
            }
        }

        [Fact]
        public async Task GetActivePlayerAdvertisementCount_ReturnsCorrectCount()
        {
            // Arrange
            var options = GetDbContextOptions("GetActivePlayerAdvertisementCount_ReturnsCorrectCount");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedPlayerAdvertisementTestBase(dbContext);
                var _playerAdvertisementRepository = new PlayerAdvertisementRepository(dbContext);

                // Act
                var result = await _playerAdvertisementRepository.GetActivePlayerAdvertisementCount();

                // Assert
                var expectedCount = await dbContext.PlayerAdvertisements
                    .Where(pa => pa.EndDate >= DateTime.UtcNow)
                    .CountAsync();

                Assert.Equal(expectedCount, result);
            }
        }

        [Fact]
        public async Task GetInactivePlayerAdvertisements_ReturnsInactiveAdvertisements()
        {
            // Arrange
            var options = GetDbContextOptions("GetInactivePlayerAdvertisements_ReturnsInactiveAdvertisements");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedPlayerAdvertisementTestBase(dbContext);
                var _playerAdvertisementRepository = new PlayerAdvertisementRepository(dbContext);

                // Act
                var result = await _playerAdvertisementRepository.GetInactivePlayerAdvertisements();

                // Assert
                Assert.NotNull(result);
                Assert.All(result, ad => Assert.True(ad.EndDate < DateTime.UtcNow));
            }
        }

        [Fact]
        public async Task CreatePlayerAdvertisement_SuccessfullyCreatesAdvertisement()
        {
            // Arrange
            var options = GetDbContextOptions("CreatePlayerAdvertisement_SuccessfullyCreatesAdvertisement");
            var newAd = new PlayerAdvertisement
            {
                Id = 1,
                PlayerPositionId = 1,
                PlayerPosition = new PlayerPosition { Id = 1, PositionName = "Goalkeeper" },
                League = "Premier League",
                Region = "England",
                Age = 30,
                Height = 197,
                PlayerFootId = 3,
                PlayerFoot = new PlayerFoot { Id = 3, FootName = "Right" },
                SalaryRangeId = 1,
                CreationDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                PlayerId = "somebody"
            };

            using (var dbContext = new AppDbContext(options))
            {
                var _playerAdvertisementRepository = new PlayerAdvertisementRepository(dbContext);

                // Act
                await _playerAdvertisementRepository.CreatePlayerAdvertisement(newAd);

                // Assert
                var result = await dbContext.PlayerAdvertisements.FindAsync(newAd.Id);
                Assert.NotNull(result);
                Assert.Equal(newAd.League, result.League);
                Assert.Equal(newAd.Region, result.Region);
                Assert.True(result.CreationDate <= DateTime.UtcNow);
                Assert.True(result.EndDate > DateTime.UtcNow);
            }
        }

        [Fact]
        public async Task UpdatePlayerAdvertisement_SuccessfullyUpdatesAdvertisement()
        {
            // Arrange
            var options = GetDbContextOptions("UpdatePlayerAdvertisement_SuccessfullyUpdatesAdvertisement");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedPlayerAdvertisementTestBase(dbContext);
                var _playerAdvertisementRepository = new PlayerAdvertisementRepository(dbContext);

                var advertisementToUpdate = await dbContext.PlayerAdvertisements.FirstAsync();
                advertisementToUpdate.League = "Serie A";
                advertisementToUpdate.Region = "Italy";

                // Act
                await _playerAdvertisementRepository.UpdatePlayerAdvertisement(advertisementToUpdate);

                // Assert
                var updatedAd = await dbContext.PlayerAdvertisements.FindAsync(advertisementToUpdate.Id);
                Assert.NotNull(updatedAd);
                Assert.Equal("Serie A", updatedAd.League);
                Assert.Equal("Italy", updatedAd.Region);
            }
        }

        [Fact]
        public async Task DeletePlayerAdvertisement_RemovesPlayerAdvertisementAndRelatedEntities()
        {
            // Arrange
            var options = GetDbContextOptions("DeletePlayerAdvertisement_RemovesPlayerAdvertisementAndRelatedEntities");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedPlayerAdvertisementTestBase(dbContext);
                await SeedClubOfferTestBase(dbContext);

                var _playerAdvertisementRepository = new PlayerAdvertisementRepository(dbContext);
                var advertisementToDelete = await dbContext.PlayerAdvertisements.FirstAsync();

                // Act
                await _playerAdvertisementRepository.DeletePlayerAdvertisement(advertisementToDelete.Id);

                // Assert
                var deletedAdvertisement = await dbContext.PlayerAdvertisements.FindAsync(advertisementToDelete.Id);
                Assert.Null(deletedAdvertisement);
            }
        }

        [Fact]
        public async Task ExportPlayerAdvertisementsToCsv_ReturnsCsvFile()
        {
            // Arrange
            var options = GetDbContextOptions("ExportPlayerAdvertisementsToCsv_ReturnsCsvFile");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedPlayerAdvertisementTestBase(dbContext);
                var _playerAdvertisementRepository = new PlayerAdvertisementRepository(dbContext);

                // Act
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
}