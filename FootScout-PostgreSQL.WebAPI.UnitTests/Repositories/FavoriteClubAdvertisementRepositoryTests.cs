using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using FootScout_PostgreSQL.WebAPI.UnitTests.TestManager;

namespace FootScout_PostgreSQL.WebAPI.UnitTests.Repositories
{
    public class FavoriteClubAdvertisementRepositoryTests : TestBase
    {
        [Fact]
        public async Task AddToFavorites_AddsAdToFavoritesToDB()
        {
            // Arrange
            var options = GetDbContextOptions("AddToFavorites_AddsAdToFavoritesToDB");
            var newFavoriteAd = new FavoriteClubAdvertisement
            {
                Id = 1,
                ClubAdvertisementId = 1,
                UserId = "leomessi"
            };

            using (var dbContext = new AppDbContext(options))
            {
                var _favoriteClubAdvertisementRepository = new FavoriteClubAdvertisementRepository(dbContext);

                // Act
                await _favoriteClubAdvertisementRepository.AddToFavorites(newFavoriteAd);

                // Assert
                var result = await dbContext.FavoriteClubAdvertisements.FindAsync(1);
                Assert.NotNull(result);
                Assert.Equal("leomessi", result.UserId);
            }
        }

        [Fact]
        public async Task DeleteFromFavorites_DeleteClubAdFromFavorites()
        {
            // Arrange
            var options = GetDbContextOptions("DeleteFromFavorites_DeleteClubAdFromFavorites");
            var favoriteClubAdvertisementId = 1;

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedClubAdvertisementTestBase(dbContext);

                var _favoriteClubAdvertisementRepository = new FavoriteClubAdvertisementRepository(dbContext);

                // Act
                await _favoriteClubAdvertisementRepository.DeleteFromFavorites(favoriteClubAdvertisementId);

                // Assert
                var result = await dbContext.FavoriteClubAdvertisements.FindAsync(favoriteClubAdvertisementId);
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task CheckClubAdvertisementIsFavorite_CheckIfClubAdvertisementIsCheckedAsFavoriteForUser()
        {
            // Arrange
            var options = GetDbContextOptions("CheckClubAdvertisementIsFavorite_CheckIfClubAdvertisementIsCheckedAsFavoriteForUser");
            var clubAdvertisementId = 1;
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedClubAdvertisementTestBase(dbContext);

                var _favoriteClubAdvertisementRepository = new FavoriteClubAdvertisementRepository(dbContext);

                // Act
                var result = await _favoriteClubAdvertisementRepository.CheckClubAdvertisementIsFavorite(clubAdvertisementId, userId);

                // Assert
                Assert.Equal(1, result);
            }
        }
    }
}