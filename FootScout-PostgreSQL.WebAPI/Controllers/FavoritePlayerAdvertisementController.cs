using AutoMapper;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Models.DTOs;
using FootScout_PostgreSQL.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_PostgreSQL.WebAPI.Controllers
{
    [Route("api/player-advertisements/favorites")]
    [Authorize(Policy = "UserRights")]
    [ApiController]
    public class FavoritePlayerAdvertisementController : ControllerBase
    {
        private readonly IFavoritePlayerAdvertisementRepository _favoritePlayerAdvertisementRepository;
        private readonly IMapper _mapper;

        public FavoritePlayerAdvertisementController(IFavoritePlayerAdvertisementRepository favoritePlayerAdvertisementRepository, IMapper mapper)
        {
            _favoritePlayerAdvertisementRepository = favoritePlayerAdvertisementRepository;
            _mapper = mapper;
        }

        // POST: api/player-advertisements/favorites
        [HttpPost]
        public async Task<ActionResult> AddToFavorites([FromBody] FavoritePlayerAdvertisementCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid dto data.");

            var favoritePlayerAdvertisement = _mapper.Map<FavoritePlayerAdvertisement>(dto);
            await _favoritePlayerAdvertisementRepository.AddToFavorites(favoritePlayerAdvertisement);

            return Ok(favoritePlayerAdvertisement);
        }

        // DELETE: api/player-advertisements/favorites/:favoritePlayerAdvertisementId
        [HttpDelete("{favoritePlayerAdvertisementId}")]
        public async Task<ActionResult> DeleteFromFavorites(int favoritePlayerAdvertisementId)
        {
            await _favoritePlayerAdvertisementRepository.DeleteFromFavorites(favoritePlayerAdvertisementId);
            return NoContent();
        }

        // GET: api/player-advertisements/favorites/check/:playerAdvertisementId/:userId
        [HttpGet("check/{playerAdvertisementId}/{userId}")]
        public async Task<IActionResult> CheckPlayerAdvertisementIsFavorite(int playerAdvertisementId, string userId)
        {
            var favoriteId = await _favoritePlayerAdvertisementRepository.CheckPlayerAdvertisementIsFavorite(playerAdvertisementId, userId);
            return Ok(favoriteId);
        }
    }
}