using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootScout_PostgreSQL.WebAPI.Repositories.Classes
{
    public class FavoriteClubAdvertisementRepository : IFavoriteClubAdvertisementRepository
    {
        private readonly AppDbContext _dbContext;

        public FavoriteClubAdvertisementRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddToFavorites(FavoriteClubAdvertisement favoriteClubAdvertisement)
        {
            await _dbContext.FavoriteClubAdvertisements.AddAsync(favoriteClubAdvertisement);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteFromFavorites(int favoriteClubAdvertisementId)
        {
            var clubAdvertisementFavorite = await _dbContext.FavoriteClubAdvertisements.FindAsync(favoriteClubAdvertisementId);
            if (clubAdvertisementFavorite == null)
                throw new ArgumentException($"No Favorite Club Advertisement found with ID {favoriteClubAdvertisementId}");

            _dbContext.FavoriteClubAdvertisements.Remove(clubAdvertisementFavorite);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CheckClubAdvertisementIsFavorite(int clubAdvertisementId, string userId)
        {
            var isFavorite = await _dbContext.FavoriteClubAdvertisements
                .FirstOrDefaultAsync(pa => pa.ClubAdvertisementId == clubAdvertisementId && pa.UserId == userId);

            return isFavorite?.Id ?? 0;
        }
    }
}