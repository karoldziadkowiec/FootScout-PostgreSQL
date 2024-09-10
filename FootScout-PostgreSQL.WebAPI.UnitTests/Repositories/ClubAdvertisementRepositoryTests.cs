using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using FootScout_PostgreSQL.WebAPI.UnitTests.TestManager;
using Microsoft.EntityFrameworkCore;

namespace FootScout_PostgreSQL.WebAPI.UnitTests.Repositories
{
    public class ClubAdvertisementRepositoryTests : TestBase
    {
        [Fact]
        public async Task GetClubAdvertisement_ReturnsCorrectClubAdvertisement()
        {
            // Arrange
            var options = GetDbContextOptions("GetClubAdvertisement_ReturnsCorrectClubAdvertisement");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedClubAdvertisementTestBase(dbContext);
                var _clubAdvertisementRepository = new ClubAdvertisementRepository(dbContext);

                // Act
                var result = await _clubAdvertisementRepository.GetClubAdvertisement(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal("Manchester City", result.ClubName);
                Assert.Equal("Premier League", result.League);
                Assert.Equal("England", result.Region);
            }
        }

        [Fact]
        public async Task GetAllClubAdvertisements_ReturnsAllClubAdvertisementsOrderedDESCByEndDate()
        {
            // Arrange
            var options = GetDbContextOptions("GetAllClubAdvertisements_ReturnsAllClubAdvertisementsOrderedDESCByEndDate");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedClubAdvertisementTestBase(dbContext);
                var _clubAdvertisementRepository = new ClubAdvertisementRepository(dbContext);

                // Act
                var result = await _clubAdvertisementRepository.GetAllClubAdvertisements();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
                Assert.True(result.First().CreationDate >= result.Last().CreationDate);
            }
        }

        [Fact]
        public async Task GetActiveClubAdvertisements_ReturnsActiveAdvertisements()
        {
            // Arrange
            var options = GetDbContextOptions("GetActiveClubAdvertisements_ReturnsActiveAdvertisements");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedClubAdvertisementTestBase(dbContext);
                var _clubAdvertisementRepository = new ClubAdvertisementRepository(dbContext);

                // Act
                var result = await _clubAdvertisementRepository.GetActiveClubAdvertisements();

                // Assert
                Assert.NotNull(result);
                Assert.All(result, ad => Assert.True(ad.EndDate >= DateTime.Now));
            }
        }

        [Fact]
        public async Task GetActiveClubAdvertisementCount_ReturnsCorrectCount()
        {
            // Arrange
            var options = GetDbContextOptions("GetActiveClubAdvertisementCount_ReturnsCorrectCount");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedClubAdvertisementTestBase(dbContext);
                var _clubAdvertisementRepository = new ClubAdvertisementRepository(dbContext);

                // Act
                var result = await _clubAdvertisementRepository.GetActiveClubAdvertisementCount();

                // Assert
                var expectedCount = await dbContext.ClubAdvertisements
                    .Where(pa => pa.EndDate >= DateTime.Now)
                    .CountAsync();

                Assert.Equal(expectedCount, result);
            }
        }

        [Fact]
        public async Task GetInactiveClubAdvertisements_ReturnsInactiveAdvertisements()
        {
            // Arrange
            var options = GetDbContextOptions("GetInactiveClubAdvertisements_ReturnsInactiveAdvertisements");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedClubAdvertisementTestBase(dbContext);
                var _clubAdvertisementRepository = new ClubAdvertisementRepository(dbContext);

                // Act
                var result = await _clubAdvertisementRepository.GetInactiveClubAdvertisements();

                // Assert
                Assert.NotNull(result);
                Assert.All(result, ad => Assert.True(ad.EndDate < DateTime.Now));
            }
        }

        [Fact]
        public async Task CreateClubAdvertisement_SuccessfullyCreatesAdvertisement()
        {
            // Arrange
            var options = GetDbContextOptions("CreateClubAdvertisement_SuccessfullyCreatesAdvertisement");
            var newAd = new ClubAdvertisement
            {
                Id = 1,
                PlayerPositionId = 1,
                ClubName = "Manchester City",
                League = "Premier League",
                Region = "England",
                SalaryRangeId = 1,
                CreationDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                ClubMemberId = "pepguardiola"
            };

            using (var dbContext = new AppDbContext(options))
            {
                var _clubAdvertisementRepository = new ClubAdvertisementRepository(dbContext);

                // Act
                await _clubAdvertisementRepository.CreateClubAdvertisement(newAd);

                // Assert
                var result = await dbContext.ClubAdvertisements.FindAsync(newAd.Id);
                Assert.NotNull(result);
                Assert.Equal(newAd.ClubName, result.ClubName);
                Assert.Equal(newAd.League, result.League);
                Assert.Equal(newAd.Region, result.Region);
                Assert.True(result.CreationDate <= DateTime.Now);
                Assert.True(result.EndDate > DateTime.Now);
            }
        }

        [Fact]
        public async Task UpdateClubAdvertisement_SuccessfullyUpdatesAdvertisement()
        {
            // Arrange
            var options = GetDbContextOptions("UpdateClubAdvertisement_SuccessfullyUpdatesAdvertisement");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedClubAdvertisementTestBase(dbContext);
                var _clubAdvertisementRepository = new ClubAdvertisementRepository(dbContext);

                var advertisementToUpdate = await dbContext.ClubAdvertisements.FirstAsync();
                advertisementToUpdate.ClubName = "Juventus";
                advertisementToUpdate.League = "Serie A";
                advertisementToUpdate.Region = "Italy";

                // Act
                await _clubAdvertisementRepository.UpdateClubAdvertisement(advertisementToUpdate);

                // Assert
                var updatedAd = await dbContext.ClubAdvertisements.FindAsync(advertisementToUpdate.Id);
                Assert.NotNull(updatedAd);
                Assert.Equal("Juventus", updatedAd.ClubName);
                Assert.Equal("Serie A", updatedAd.League);
                Assert.Equal("Italy", updatedAd.Region);
            }
        }

        [Fact]
        public async Task DeleteClubAdvertisement_RemovesClubAdvertisementAndRelatedEntities()
        {
            // Arrange
            var options = GetDbContextOptions("DeleteClubAdvertisement_RemovesClubAdvertisementAndRelatedEntities");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedClubAdvertisementTestBase(dbContext);
                await SeedPlayerOfferTestBase(dbContext);

                var _clubAdvertisementRepository = new ClubAdvertisementRepository(dbContext);
                var advertisementToDelete = await dbContext.ClubAdvertisements.FirstAsync();

                // Act
                await _clubAdvertisementRepository.DeleteClubAdvertisement(advertisementToDelete.Id);

                // Assert
                var deletedAdvertisement = await dbContext.ClubAdvertisements.FindAsync(advertisementToDelete.Id);
                Assert.Null(deletedAdvertisement);
            }
        }

        [Fact]
        public async Task ExportClubAdvertisementsToCsv_ReturnsCsvFile()
        {
            // Arrange
            var options = GetDbContextOptions("ExportClubAdvertisementsToCsv_ReturnsCsvFile");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedClubAdvertisementTestBase(dbContext);
                var _clubAdvertisementRepository = new ClubAdvertisementRepository(dbContext);

                // Act
                var csvStream = await _clubAdvertisementRepository.ExportClubAdvertisementsToCsv();
                csvStream.Position = 0;

                using (var reader = new StreamReader(csvStream))
                {
                    var csvContent = await reader.ReadToEndAsync();

                    // Assert
                    Assert.NotEmpty(csvContent);
                    Assert.Contains("E-mail,First Name,Last Name,Position,Club Name,League,Region,Min Salary,Max Salary,Creation Date,End Date", csvContent);
                    Assert.Contains("pg8@gmail.com,Pep,Guardiola,Striker", csvContent);
                }
            }
        }
    }
}