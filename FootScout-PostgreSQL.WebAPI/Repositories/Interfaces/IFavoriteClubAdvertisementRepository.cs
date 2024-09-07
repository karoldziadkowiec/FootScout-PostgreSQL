using FootScout_PostgreSQL.WebAPI.Entities;

namespace FootScout_PostgreSQL.WebAPI.Repositories.Interfaces
{
    public interface IFavoriteClubAdvertisementRepository
    {
        Task AddToFavorites(FavoriteClubAdvertisement favoriteClubAdvertisement);
        Task DeleteFromFavorites(int favoriteClubAdvertisementId);
        Task<int> CheckClubAdvertisementIsFavorite(int clubAdvertisementId, string userId);
    }
}