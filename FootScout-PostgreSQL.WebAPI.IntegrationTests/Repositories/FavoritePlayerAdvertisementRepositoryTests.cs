using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using Microsoft.EntityFrameworkCore;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Repositories
{
    public class FavoritePlayerAdvertisementRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly AppDbContext _dbContext;
        private readonly FavoritePlayerAdvertisementRepository _favoritePlayerAdvertisementRepository;
        public FavoritePlayerAdvertisementRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _favoritePlayerAdvertisementRepository = new FavoritePlayerAdvertisementRepository(_dbContext);
        }

        [Fact]
        public async Task AddToFavorites_AddsAdToFavoritesToDB()
        {
            // Arrange
            var newFavoriteAd = new FavoritePlayerAdvertisement
            {
                PlayerAdvertisementId = 1,
                UserId = "leomessi"
            };

            // Act
            await _favoritePlayerAdvertisementRepository.AddToFavorites(newFavoriteAd);

            var result = await _dbContext.FavoritePlayerAdvertisements
                .FirstOrDefaultAsync(pa => pa.PlayerAdvertisementId == 1 && pa.UserId == "leomessi");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.PlayerAdvertisementId);
            Assert.Equal("leomessi", result.UserId);

            _dbContext.FavoritePlayerAdvertisements.Remove(result);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteFromFavorites_DeletePlayerAdFromFavorites()
        {
            // Arrange
            var newFavoriteAd = new FavoritePlayerAdvertisement
            {
                PlayerAdvertisementId = 2,
                UserId = "leomessi"
            };
            await _favoritePlayerAdvertisementRepository.AddToFavorites(newFavoriteAd);

            var favResult = await _dbContext.FavoritePlayerAdvertisements
                .FirstOrDefaultAsync(pa => pa.PlayerAdvertisementId == 2 && pa.UserId == "leomessi");

            // Act
            await _favoritePlayerAdvertisementRepository.DeleteFromFavorites(favResult.Id);

            // Assert
            var result = await _dbContext.FavoritePlayerAdvertisements.FindAsync(favResult.Id);
            Assert.Null(result);
        }

        [Fact]
        public async Task CheckPlayerAdvertisementIsFavorite_CheckIfPlayerAdvertisementIsCheckedAsFavoriteForUser()
        {
            // Arrange
            var playerAdvertisementId = 1;
            var userId = "pepguardiola";

            // Act
            var result = await _favoritePlayerAdvertisementRepository.CheckPlayerAdvertisementIsFavorite(playerAdvertisementId, userId);

            // Assert
            Assert.Equal(1, result);
        }
    }
}