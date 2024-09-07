using AutoMapper;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Models.DTOs;
using FootScout_PostgreSQL.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_PostgreSQL.WebAPI.Controllers
{
    [Route("api/club-advertisements/favorites")]
    [Authorize(Policy = "UserRights")]
    [ApiController]
    public class FavoriteClubAdvertisementController : ControllerBase
    {
        private readonly IFavoriteClubAdvertisementRepository _favoriteAdvertisementRepository;
        private readonly IMapper _mapper;

        public FavoriteClubAdvertisementController(IFavoriteClubAdvertisementRepository favoriteAdvertisementRepository, IMapper mapper)
        {
            _favoriteAdvertisementRepository = favoriteAdvertisementRepository;
            _mapper = mapper;
        }

        // POST: api/club-advertisements/favorites
        [HttpPost]
        public async Task<ActionResult> AddToFavorites([FromBody] FavoriteClubAdvertisementCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid dto data.");

            var favoriteClubAdvertisement = _mapper.Map<FavoriteClubAdvertisement>(dto);
            await _favoriteAdvertisementRepository.AddToFavorites(favoriteClubAdvertisement);

            return Ok(favoriteClubAdvertisement);
        }

        // DELETE: api/club-advertisements/favorites/:favoriteClubAdvertisementId
        [HttpDelete("{favoriteClubAdvertisementId}")]
        public async Task<ActionResult> DeleteFromFavorites(int favoriteClubAdvertisementId)
        {
            await _favoriteAdvertisementRepository.DeleteFromFavorites(favoriteClubAdvertisementId);
            return NoContent();
        }

        // GET: api/club-advertisements/favorites/check/:clubAdvertisementId/:userId
        [HttpGet("check/{clubAdvertisementId}/{userId}")]
        public async Task<IActionResult> CheckClubAdvertisementIsFavorite(int clubAdvertisementId, string userId)
        {
            var favoriteId = await _favoriteAdvertisementRepository.CheckClubAdvertisementIsFavorite(clubAdvertisementId, userId);
            return Ok(favoriteId);
        }
    }
}