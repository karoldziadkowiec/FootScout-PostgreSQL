using FootScout_PostgreSQL.WebAPI.Entities;

namespace FootScout_PostgreSQL.WebAPI.Repositories.Interfaces
{
    public interface IFavoritePlayerAdvertisementRepository
    {
        Task AddToFavorites(FavoritePlayerAdvertisement favoritePlayerAdvertisement);
        Task DeleteFromFavorites(int favoritePlayerAdvertisementId);
        Task<int> CheckPlayerAdvertisementIsFavorite(int playerAdvertisementId, string userId);
    }
}