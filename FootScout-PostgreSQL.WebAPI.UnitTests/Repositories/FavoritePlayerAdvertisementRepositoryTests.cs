using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using FootScout_PostgreSQL.WebAPI.UnitTests.TestManager;

namespace FootScout_PostgreSQL.WebAPI.UnitTests.Repositories
{
    public class FavoritePlayerAdvertisementRepositoryTests : TestBase
    {
        [Fact]
        public async Task AddToFavorites_AddsAdToFavoritesToDB()
        {
            // Arrange
            var options = GetDbContextOptions("AddToFavorites_AddsAdToFavoritesToDB");
            var newFavoriteAd = new FavoritePlayerAdvertisement
            {
                Id = 1,
                PlayerAdvertisementId = 1,
                UserId = "pepguardiola"
            };

            using (var dbContext = new AppDbContext(options))
            {
                var _favoritePlayerAdvertisementRepository = new FavoritePlayerAdvertisementRepository(dbContext);

                // Act
                await _favoritePlayerAdvertisementRepository.AddToFavorites(newFavoriteAd);

                // Assert
                var result = await dbContext.FavoritePlayerAdvertisements.FindAsync(1);
                Assert.NotNull(result);
                Assert.Equal("pepguardiola", result.UserId);
            }
        }

        [Fact]
        public async Task DeleteFromFavorites_DeletePlayerAdFromFavorites()
        {
            // Arrange
            var options = GetDbContextOptions("DeleteFromFavorites_DeletePlayerAdFromFavorites");
            var favoritePlayerAdvertisementId = 1;

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerAdvertisementTestBase(dbContext);

                var _favoritePlayerAdvertisementRepository = new FavoritePlayerAdvertisementRepository(dbContext);

                // Act
                await _favoritePlayerAdvertisementRepository.DeleteFromFavorites(favoritePlayerAdvertisementId);

                // Assert
                var result = await dbContext.FavoritePlayerAdvertisements.FindAsync(favoritePlayerAdvertisementId);
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task CheckPlayerAdvertisementIsFavorite_CheckIfPlayerAdvertisementIsCheckedAsFavoriteForUser()
        {
            // Arrange
            var options = GetDbContextOptions("CheckPlayerAdvertisementIsFavorite_CheckIfPlayerAdvertisementIsCheckedAsFavoriteForUser");
            var playerAdvertisementId = 1;
            var userId = "pepguardiola";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerAdvertisementTestBase(dbContext);

                var _favoritePlayerAdvertisementRepository = new FavoritePlayerAdvertisementRepository(dbContext);

                // Act
                var result = await _favoritePlayerAdvertisementRepository.CheckPlayerAdvertisementIsFavorite(playerAdvertisementId, userId);

                // Assert
                Assert.Equal(1, result);
            }
        }
    }
}