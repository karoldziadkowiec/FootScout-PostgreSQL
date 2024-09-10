﻿using AutoMapper;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Models.DTOs;
using FootScout_PostgreSQL.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_PostgreSQL.WebAPI.Controllers
{
    [Route("api/player-advertisements")]
    [Authorize(Policy = "AdminOrUserRights")]
    [ApiController]
    public class PlayerAdvertisementController : ControllerBase
    {
        private readonly IPlayerAdvertisementRepository _playerAdvertisementRepository;
        private readonly ISalaryRangeRepository _salaryRangeRepository;
        private readonly IPlayerPositionRepository _playerPositionRepository;
        private readonly IPlayerFootRepository _playerFootRepository;
        private readonly IMapper _mapper;

        public PlayerAdvertisementController(IPlayerAdvertisementRepository playerAdvertisementRepository, ISalaryRangeRepository salaryRangeRepository, IPlayerPositionRepository playerPositionRepository, IPlayerFootRepository playerFootRepository, IMapper mapper)
        {
            _playerAdvertisementRepository = playerAdvertisementRepository;
            _salaryRangeRepository = salaryRangeRepository;
            _playerPositionRepository = playerPositionRepository;
            _playerFootRepository = playerFootRepository;
            _mapper = mapper;
        }

        // GET: api/player-advertisements/:playerAdvertisementId
        [HttpGet("{playerAdvertisementId}")]
        public async Task<ActionResult<PlayerAdvertisement>> GetPlayerAdvertisement(int playerAdvertisementId)
        {
            var playerAdvertisement = await _playerAdvertisementRepository.GetPlayerAdvertisement(playerAdvertisementId);
            if (playerAdvertisement == null)
                return NotFound();

            return Ok(playerAdvertisement);
        }

        // GET: api/player-advertisements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerAdvertisement>>> GetAllPlayerAdvertisements()
        {
            var playerAdvertisements = await _playerAdvertisementRepository.GetAllPlayerAdvertisements();
            return Ok(playerAdvertisements);
        }

        // GET: api/player-advertisements/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<PlayerAdvertisement>>> GetActivePlayerAdvertisements()
        {
            var activePlayerAdvertisements = await _playerAdvertisementRepository.GetActivePlayerAdvertisements();
            return Ok(activePlayerAdvertisements);
        }

        // GET: api/player-advertisements/active/count
        [HttpGet("active/count")]
        public async Task<IActionResult> GetActivePlayerAdvertisementCount()
        {
            int count = await _playerAdvertisementRepository.GetActivePlayerAdvertisementCount();
            return Ok(count);
        }

        // GET: api/player-advertisements/inactive
        [HttpGet("inactive")]
        public async Task<ActionResult<IEnumerable<PlayerAdvertisement>>> GetInactivePlayerAdvertisements()
        {
            var inactivePlayerAdvertisements = await _playerAdvertisementRepository.GetInactivePlayerAdvertisements();
            return Ok(inactivePlayerAdvertisements);
        }

        // POST: api/player-advertisements
        [HttpPost]
        public async Task<ActionResult> CreatePlayerAdvertisement([FromBody] PlayerAdvertisementCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid dto data.");

            var salaryRange = _mapper.Map<SalaryRange>(dto.SalaryRangeDTO);
            await _salaryRangeRepository.CreateSalaryRange(salaryRange);

            var playerAdvertisement = _mapper.Map<PlayerAdvertisement>(dto);
            playerAdvertisement.SalaryRangeId = salaryRange.Id;

            if (playerAdvertisement.PlayerPositionId != 0)
                playerAdvertisement.PlayerPosition = await _playerPositionRepository.GetPlayerPosition(playerAdvertisement.PlayerPositionId);

            if (playerAdvertisement.PlayerFootId != 0)
                playerAdvertisement.PlayerFoot = await _playerFootRepository.GetPlayerFoot(playerAdvertisement.PlayerFootId);

            await _playerAdvertisementRepository.CreatePlayerAdvertisement(playerAdvertisement);
            return Ok(playerAdvertisement);
        }

        // PUT: api/player-advertisements/:playerAdvertisementId
        [HttpPut("{playerAdvertisementId}")]
        public async Task<ActionResult> UpdatePlayerAdvertisement(int playerAdvertisementId, [FromBody] PlayerAdvertisement playerAdvertisement)
        {
            if (playerAdvertisementId != playerAdvertisement.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (playerAdvertisement.PlayerPositionId != 0)
                playerAdvertisement.PlayerPosition = await _playerPositionRepository.GetPlayerPosition(playerAdvertisement.PlayerPositionId);

            if (playerAdvertisement.PlayerFootId != 0)
                playerAdvertisement.PlayerFoot = await _playerFootRepository.GetPlayerFoot(playerAdvertisement.PlayerFootId);

            await _playerAdvertisementRepository.UpdatePlayerAdvertisement(playerAdvertisement);
            return NoContent();
        }

        // DELETE: api/player-advertisements/:playerAdvertisementId
        [HttpDelete("{playerAdvertisementId}")]
        public async Task<ActionResult> DeletePlayerAdvertisement(int playerAdvertisementId)
        {
            var playerAdvertisement = await _playerAdvertisementRepository.GetPlayerAdvertisement(playerAdvertisementId);
            if (playerAdvertisement == null)
                return NotFound();

            await _playerAdvertisementRepository.DeletePlayerAdvertisement(playerAdvertisementId);
            return NoContent();
        }

        // GET: api/player-advertisements/export
        [HttpGet("export")]
        public async Task<IActionResult> ExportPlayerAdvertisementsToCsv()
        {
            var csvStream = await _playerAdvertisementRepository.ExportPlayerAdvertisementsToCsv();
            return File(csvStream, "text/csv", "player-advertisements.csv");
        }
    }
}