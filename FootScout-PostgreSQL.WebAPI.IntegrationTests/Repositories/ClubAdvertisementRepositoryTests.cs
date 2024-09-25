using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using Microsoft.EntityFrameworkCore;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Repositories
{
    public class ClubAdvertisementRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly AppDbContext _dbContext;
        private readonly ClubAdvertisementRepository _clubAdvertisementRepository;

        public ClubAdvertisementRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _clubAdvertisementRepository = new ClubAdvertisementRepository(_dbContext);
        }

        [Fact]
        public async Task GetClubAdvertisement_ReturnsCorrectClubAdvertisement()
        {
            // Arrange & Act
            var result = await _clubAdvertisementRepository.GetClubAdvertisement(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Manchester City", result.ClubName);
            Assert.Equal("Premier League", result.League);
            Assert.Equal("England", result.Region);
        }

        [Fact]
        public async Task GetAllClubAdvertisements_ReturnsAllClubAdvertisementsOrderedDESCByEndDate()
        {
            // Arrange & Act
            var result = await _clubAdvertisementRepository.GetAllClubAdvertisements();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.True(result.First().CreationDate >= result.Last().CreationDate);
        }

        [Fact]
        public async Task GetActiveClubAdvertisements_ReturnsActiveAdvertisements()
        {
            // Arrange & Act
            var result = await _clubAdvertisementRepository.GetActiveClubAdvertisements();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, ad => Assert.True(ad.EndDate >= DateTime.UtcNow));
        }

        [Fact]
        public async Task GetActiveClubAdvertisementCount_ReturnsCorrectCount()
        {
            // Arrange & Act
            var result = await _clubAdvertisementRepository.GetActiveClubAdvertisementCount();

            // Assert
            var expectedCount = await _dbContext.ClubAdvertisements
                .Where(pa => pa.EndDate >= DateTime.UtcNow)
                .CountAsync();

            Assert.Equal(expectedCount, result);
        }

        [Fact]
        public async Task GetInactiveClubAdvertisements_ReturnsInactiveAdvertisements()
        {
            // Arrange & Act
            var result = await _clubAdvertisementRepository.GetInactiveClubAdvertisements();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, ad => Assert.True(ad.EndDate < DateTime.UtcNow));
        }

        [Fact]
        public async Task CreateClubAdvertisement_SuccessfullyCreatesAdvertisement()
        {
            // Arrange
            _dbContext.SalaryRanges.Add(new SalaryRange { Min = 550, Max = 600 });
            await _dbContext.SaveChangesAsync();

            var salaryRange = await _dbContext.SalaryRanges
                .FirstOrDefaultAsync(sr => sr.Min == 550 && sr.Max == 600);

            var newAd = new ClubAdvertisement
            {
                PlayerPositionId = 1,
                PlayerPosition = new PlayerPosition { Id = 1, PositionName = "Goalkeeper" },
                ClubName = "Juventus Turyn",
                League = "Serie A",
                Region = "Italy",
                SalaryRangeId = salaryRange.Id,
                CreationDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                ClubMemberId = "pepguardiola"
            };

            // Act
            await _clubAdvertisementRepository.CreateClubAdvertisement(newAd);

            var result = await _dbContext.ClubAdvertisements.FindAsync(newAd.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newAd.ClubName, result.ClubName);
            Assert.Equal(newAd.League, result.League);
            Assert.Equal(newAd.Region, result.Region);
            Assert.True(result.CreationDate <= DateTime.UtcNow);
            Assert.True(result.EndDate > DateTime.UtcNow);

            _dbContext.ClubAdvertisements.Remove(result);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateClubAdvertisement_SuccessfullyUpdatesAdvertisement()
        {
            // Arrange
            var advertisementToUpdate = await _dbContext.ClubAdvertisements.FirstAsync();
            advertisementToUpdate.PlayerPositionId = 12;

            // Act
            await _clubAdvertisementRepository.UpdateClubAdvertisement(advertisementToUpdate);

            // Assert
            var updatedAd = await _dbContext.ClubAdvertisements.FindAsync(advertisementToUpdate.Id);
            Assert.NotNull(updatedAd);
            Assert.Equal(12, updatedAd.PlayerPositionId);
        }

        [Fact]
        public async Task DeleteClubAdvertisement_RemovesClubAdvertisementAndRelatedEntities()
        {
            // Arrange
            _dbContext.SalaryRanges.Add(new SalaryRange { Min = 650, Max = 700 });
            await _dbContext.SaveChangesAsync();

            var salaryRange = await _dbContext.SalaryRanges
                .FirstOrDefaultAsync(sr => sr.Min == 650 && sr.Max == 700);

            _dbContext.ClubAdvertisements.Add(new ClubAdvertisement
            {
                PlayerPositionId = 1,
                PlayerPosition = new PlayerPosition { Id = 1, PositionName = "Goalkeeper" },
                ClubName = "Bayern Monachium",
                League = "Bundesliga",
                Region = "Germany",
                SalaryRangeId = salaryRange.Id,
                CreationDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                ClubMemberId = "pepguardiola"
            });
            await _dbContext.SaveChangesAsync();

            var advertisementToDelete = await _dbContext.ClubAdvertisements
                .FirstOrDefaultAsync(pa => pa.League == "Bundesliga" && pa.Region == "Germany" && pa.ClubMemberId == "pepguardiola");

            // Act
            await _clubAdvertisementRepository.DeleteClubAdvertisement(advertisementToDelete.Id);

            // Assert
            var deletedAdvertisement = await _dbContext.ClubAdvertisements.FindAsync(advertisementToDelete.Id);
            Assert.Null(deletedAdvertisement);
        }

        [Fact]
        public async Task ExportClubAdvertisementsToCsv_ReturnsCsvFile()
        {
            // Arrange & Act
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