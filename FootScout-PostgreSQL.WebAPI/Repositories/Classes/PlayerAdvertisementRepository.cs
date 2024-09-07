using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FootScout_PostgreSQL.WebAPI.Repositories.Classes
{
    public class PlayerAdvertisementRepository : IPlayerAdvertisementRepository
    {
        private readonly AppDbContext _dbContext;

        public PlayerAdvertisementRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PlayerAdvertisement> GetPlayerAdvertisement(int playerAdvertisementId)
        {
            return await _dbContext.PlayerAdvertisements
                .Include(pa => pa.PlayerPosition)
                .Include(pa => pa.PlayerFoot)
                .Include(pa => pa.SalaryRange)
                .Include(pa => pa.Player)
                .FirstOrDefaultAsync(pa => pa.Id == playerAdvertisementId);
        }

        public async Task<IEnumerable<PlayerAdvertisement>> GetAllPlayerAdvertisements()
        {
            return await _dbContext.PlayerAdvertisements
                .Include(pa => pa.PlayerPosition)
                .Include(pa => pa.PlayerFoot)
                .Include(pa => pa.SalaryRange)
                .Include(pa => pa.Player)
                .OrderByDescending(pa => pa.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlayerAdvertisement>> GetActivePlayerAdvertisements()
        {
            return await _dbContext.PlayerAdvertisements
                .Include(pa => pa.PlayerPosition)
                .Include(pa => pa.PlayerFoot)
                .Include(pa => pa.SalaryRange)
                .Include(pa => pa.Player)
                .Where(pa => pa.EndDate >= DateTime.UtcNow)
                .OrderByDescending(pa => pa.EndDate)
                .ToListAsync();
        }

        public async Task<int> GetActivePlayerAdvertisementCount()
        {
            return await _dbContext.PlayerAdvertisements.Where(pa => pa.EndDate >= DateTime.UtcNow).CountAsync();
        }

        public async Task<IEnumerable<PlayerAdvertisement>> GetInactivePlayerAdvertisements()
        {
            return await _dbContext.PlayerAdvertisements
                .Include(pa => pa.PlayerPosition)
                .Include(pa => pa.PlayerFoot)
                .Include(pa => pa.SalaryRange)
                .Include(pa => pa.Player)
                .Where(pa => pa.EndDate < DateTime.UtcNow)
                .OrderByDescending(pa => pa.EndDate)
                .ToListAsync();
        }

        public async Task CreatePlayerAdvertisement(PlayerAdvertisement playerAdvertisement)
        {
            playerAdvertisement.CreationDate = DateTime.UtcNow;
            playerAdvertisement.EndDate = DateTime.UtcNow.AddDays(30);

            await _dbContext.PlayerAdvertisements.AddAsync(playerAdvertisement);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdatePlayerAdvertisement(PlayerAdvertisement playerAdvertisement)
        {
            _dbContext.PlayerAdvertisements.Update(playerAdvertisement);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePlayerAdvertisement(int playerAdvertisementId)
        {
            var playerAdvertisement = await _dbContext.PlayerAdvertisements.FindAsync(playerAdvertisementId);
            if (playerAdvertisement == null)
                throw new ArgumentException($"No Player Advertisement found with ID {playerAdvertisementId}");

            if (playerAdvertisement.SalaryRangeId != null)
            {
                var salaryRange = await _dbContext.SalaryRanges.FindAsync(playerAdvertisement.SalaryRangeId);
                if (salaryRange != null)
                    _dbContext.SalaryRanges.Remove(salaryRange);
            }

            var favorites = await _dbContext.FavoritePlayerAdvertisements
                .Where(pa => pa.PlayerAdvertisementId == playerAdvertisementId)
                .ToListAsync();
            _dbContext.FavoritePlayerAdvertisements.RemoveRange(favorites);

            var clubOffers = await _dbContext.ClubOffers
                .Where(co => co.PlayerAdvertisementId == playerAdvertisementId)
                .ToListAsync();
            _dbContext.ClubOffers.RemoveRange(clubOffers);

            _dbContext.PlayerAdvertisements.Remove(playerAdvertisement);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<MemoryStream> ExportPlayerAdvertisementsToCsv()
        {
            var playerAdvertisements = await GetAllPlayerAdvertisements();
            var csv = new StringBuilder();
            csv.AppendLine("E-mail,First Name,Last Name,Position,League,Region,Age,Height,Foot,Min Salary,Max Salary,Creation Date,End Date");

            foreach (var ad in playerAdvertisements)
            {
                csv.AppendLine($"{ad.Player.Email},{ad.Player.FirstName},{ad.Player.LastName},{ad.PlayerPosition.PositionName},{ad.League},{ad.Region},{ad.Age},{ad.Height},{ad.PlayerFoot.FootName},{ad.SalaryRange.Min},{ad.SalaryRange.Max},{ad.CreationDate:yyyy-MM-dd},{ad.EndDate:yyyy-MM-dd}");
            }

            var byteArray = Encoding.UTF8.GetBytes(csv.ToString());
            var csvStream = new MemoryStream(byteArray);

            return csvStream;
        }
    }
}