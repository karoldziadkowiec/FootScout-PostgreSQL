﻿using AutoMapper;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Models.DTOs;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using FootScout_PostgreSQL.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_PostgreSQL.WebAPI.Controllers
{
    [Route("api/club-advertisements")]
    [Authorize(Policy = "AdminOrUserRights")]
    [ApiController]
    public class ClubAdvertisementController : ControllerBase
    {
        private readonly IClubAdvertisementRepository _clubAdvertisementRepository;
        private readonly ISalaryRangeRepository _salaryRangeRepository;
        private readonly IPlayerPositionRepository _playerPositionRepository;
        private readonly IMapper _mapper;

        public ClubAdvertisementController(IClubAdvertisementRepository clubAdvertisementRepository, ISalaryRangeRepository salaryRangeRepository, IPlayerPositionRepository playerPositionRepository, IMapper mapper)
        {
            _clubAdvertisementRepository = clubAdvertisementRepository;
            _salaryRangeRepository = salaryRangeRepository;
            _playerPositionRepository = playerPositionRepository;
            _mapper = mapper;
        }

        // GET: api/club-advertisements/:clubAdvertisementId
        [HttpGet("{clubAdvertisementId}")]
        public async Task<ActionResult<PlayerAdvertisement>> GetClubAdvertisement(int clubAdvertisementId)
        {
            var clubAdvertisement = await _clubAdvertisementRepository.GetClubAdvertisement(clubAdvertisementId);
            if (clubAdvertisement == null)
                return NotFound();

            return Ok(clubAdvertisement);
        }

        // GET: api/club-advertisements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerAdvertisement>>> GetAllClubAdvertisements()
        {
            var clubAdvertisements = await _clubAdvertisementRepository.GetAllClubAdvertisements();
            return Ok(clubAdvertisements);
        }

        // GET: api/club-advertisements/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<PlayerAdvertisement>>> GetActiveClubAdvertisements()
        {
            var activeClubAdvertisements = await _clubAdvertisementRepository.GetActiveClubAdvertisements();
            return Ok(activeClubAdvertisements);
        }

        // GET: api/club-advertisements/active/count
        [HttpGet("active/count")]
        public async Task<IActionResult> GetActiveClubAdvertisementCount()
        {
            int count = await _clubAdvertisementRepository.GetActiveClubAdvertisementCount();
            return Ok(count);
        }

        // GET: api/club-advertisements/inactive
        [HttpGet("inactive")]
        public async Task<ActionResult<IEnumerable<PlayerAdvertisement>>> GetInactiveClubAdvertisements()
        {
            var inactiveClubAdvertisements = await _clubAdvertisementRepository.GetInactiveClubAdvertisements();
            return Ok(inactiveClubAdvertisements);
        }

        // POST: api/club-advertisements
        [HttpPost]
        public async Task<ActionResult> CreateClubAdvertisement([FromBody] ClubAdvertisementCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid dto data.");

            var salaryRange = _mapper.Map<SalaryRange>(dto.SalaryRangeDTO);
            await _salaryRangeRepository.CreateSalaryRange(salaryRange);

            var clubAdvertisement = _mapper.Map<ClubAdvertisement>(dto);
            clubAdvertisement.SalaryRangeId = salaryRange.Id;

            if (clubAdvertisement.PlayerPositionId != 0)
                clubAdvertisement.PlayerPosition = await _playerPositionRepository.GetPlayerPosition(clubAdvertisement.PlayerPositionId);

            await _clubAdvertisementRepository.CreateClubAdvertisement(clubAdvertisement);
            return Ok(clubAdvertisement);
        }

        // PUT: api/club-advertisements/:clubAdvertisementId
        [HttpPut("{clubAdvertisementId}")]
        public async Task<ActionResult> UpdateClubAdvertisement(int clubAdvertisementId, [FromBody] ClubAdvertisement clubAdvertisement)
        {
            if (clubAdvertisementId != clubAdvertisement.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (clubAdvertisement.PlayerPositionId != 0)
                clubAdvertisement.PlayerPosition = await _playerPositionRepository.GetPlayerPosition(clubAdvertisement.PlayerPositionId);

            await _clubAdvertisementRepository.UpdateClubAdvertisement(clubAdvertisement);
            return NoContent();
        }

        // DELETE: api/club-advertisements/:clubAdvertisementId
        [HttpDelete("{clubAdvertisementId}")]
        public async Task<ActionResult> DeleteClubAdvertisement(int clubAdvertisementId)
        {
            var clubAdvertisement = await _clubAdvertisementRepository.GetClubAdvertisement(clubAdvertisementId);
            if (clubAdvertisement == null)
                return NotFound();

            await _clubAdvertisementRepository.DeleteClubAdvertisement(clubAdvertisementId);
            return NoContent();
        }

        // GET: api/club-advertisements/export
        [HttpGet("export")]
        public async Task<IActionResult> ExportClubAdvertisementsToCsv()
        {
            var csvStream = await _clubAdvertisementRepository.ExportClubAdvertisementsToCsv();
            return File(csvStream, "text/csv", "club-advertisements.csv");
        }
    }
}