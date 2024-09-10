using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using FootScout_PostgreSQL.WebAPI.UnitTests.TestManager;

namespace FootScout_PostgreSQL.WebAPI.UnitTests.Repositories
{
    public class OfferStatusRepositoryTests : TestBase
    {
        [Fact]
        public async Task GetOfferStatuses_ReturnsAllOfferStatuses()
        {
            // Arrange
            var options = GetDbContextOptions("GetOfferStatuses_ReturnsAllOfferStatuses");

            using (var dbContext = new AppDbContext(options))
            {
                await SeedOfferStatusTestBase(dbContext);
                var _offerStatusRepository = new OfferStatusRepository(dbContext);

                // Act
                var result = await _offerStatusRepository.GetOfferStatuses();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(3, result.Count());
            }
        }

        [Fact]
        public async Task GetOfferStatus_ReturnsCorrectOfferStatus()
        {
            // Arrange
            var options = GetDbContextOptions("GetOfferStatus_ReturnsCorrectOfferStatus");

            using (var dbContext = new AppDbContext(options))
            {
                await SeedOfferStatusTestBase(dbContext);
                var _offerStatusRepository = new OfferStatusRepository(dbContext);

                // Act
                var result = await _offerStatusRepository.GetOfferStatus(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal("Offered", result.StatusName);
            }
        }

        [Fact]
        public async Task GetOfferStatus_ReturnsNull_WhenStatusDoesNotExist()
        {
            // Arrange
            var options = GetDbContextOptions("GetOfferStatus_ReturnsNull_WhenStatusDoesNotExist");

            using (var dbContext = new AppDbContext(options))
            {
                await SeedOfferStatusTestBase(dbContext);
                var _offerStatusRepository = new OfferStatusRepository(dbContext);

                // Act
                var result = await _offerStatusRepository.GetOfferStatus(100);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetOfferStatusName_ReturnsCorrectStatusName()
        {
            // Arrange
            var options = GetDbContextOptions("GetOfferStatusName_ReturnsCorrectStatusName");

            using (var dbContext = new AppDbContext(options))
            {
                await SeedOfferStatusTestBase(dbContext);
                var _offerStatusRepository = new OfferStatusRepository(dbContext);

                // Act
                var result = await _offerStatusRepository.GetOfferStatusName(1);

                // Assert
                Assert.Equal("Offered", result);
            }
        }

        [Fact]
        public async Task GetOfferStatusName_ReturnsNull_WhenStatusIdDoesNotExist()
        {
            // Arrange
            var options = GetDbContextOptions("GetOfferStatusName_ReturnsNull_WhenStatusIdDoesNotExist");

            using (var dbContext = new AppDbContext(options))
            {
                await SeedOfferStatusTestBase(dbContext);
                var _offerStatusRepository = new OfferStatusRepository(dbContext);

                // Act
                var result = await _offerStatusRepository.GetOfferStatusName(100);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetOfferStatusId_ReturnsCorrectStatusId()
        {
            // Arrange
            var options = GetDbContextOptions("GetOfferStatusId_ReturnsCorrectStatusId");

            using (var dbContext = new AppDbContext(options))
            {
                await SeedOfferStatusTestBase(dbContext);
                var _offerStatusRepository = new OfferStatusRepository(dbContext);

                // Act
                var result = await _offerStatusRepository.GetOfferStatusId("Accepted");

                // Assert
                Assert.Equal(2, result);
            }
        }

        [Fact]
        public async Task GetOfferStatusId_ReturnsZero_WhenStatusNameDoesNotExist()
        {
            // Arrange
            var options = GetDbContextOptions("GetOfferStatusId_ReturnsZero_WhenStatusNameDoesNotExist");

            using (var dbContext = new AppDbContext(options))
            {
                await SeedOfferStatusTestBase(dbContext);
                var _offerStatusRepository = new OfferStatusRepository(dbContext);

                // Act
                var result = await _offerStatusRepository.GetOfferStatusId("NonExistentStatus");

                // Assert
                Assert.Equal(0, result);
            }
        }
    }
}