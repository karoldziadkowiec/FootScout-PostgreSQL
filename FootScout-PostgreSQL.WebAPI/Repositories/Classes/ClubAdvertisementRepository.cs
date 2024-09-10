using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FootScout_PostgreSQL.WebAPI.Repositories.Classes
{
    public class ClubAdvertisementRepository : IClubAdvertisementRepository
    {
        private readonly AppDbContext _dbContext;

        public ClubAdvertisementRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ClubAdvertisement> GetClubAdvertisement(int clubAdvertisementId)
        {
            return await _dbContext.ClubAdvertisements
                .Include(ca => ca.SalaryRange)
                .Include(ca => ca.ClubMember)
                .FirstOrDefaultAsync(ca => ca.Id == clubAdvertisementId);
        }

        public async Task<IEnumerable<ClubAdvertisement>> GetAllClubAdvertisements()
        {
            return await _dbContext.ClubAdvertisements
                .Include(ca => ca.SalaryRange)
                .Include(ca => ca.ClubMember)
                .OrderByDescending(ca => ca.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClubAdvertisement>> GetActiveClubAdvertisements()
        {
            return await _dbContext.ClubAdvertisements
                .Include(ca => ca.SalaryRange)
                .Include(ca => ca.ClubMember)
                .Where(ca => ca.EndDate >= DateTime.UtcNow)
                .OrderByDescending(ca => ca.EndDate)
                .ToListAsync();
        }

        public async Task<int> GetActiveClubAdvertisementCount()
        {
            return await _dbContext.ClubAdvertisements.Where(ca => ca.EndDate >= DateTime.UtcNow).CountAsync();
        }

        public async Task<IEnumerable<ClubAdvertisement>> GetInactiveClubAdvertisements()
        {
            return await _dbContext.ClubAdvertisements
                .Include(ca => ca.SalaryRange)
                .Include(ca => ca.ClubMember)
                .Where(ca => ca.EndDate < DateTime.UtcNow)
                .OrderByDescending(ca => ca.EndDate)
                .ToListAsync();
        }

        public async Task CreateClubAdvertisement(ClubAdvertisement clubAdvertisement)
        {
            clubAdvertisement.CreationDate = DateTime.UtcNow;
            clubAdvertisement.EndDate = DateTime.UtcNow.AddDays(30);

            await _dbContext.ClubAdvertisements.AddAsync(clubAdvertisement);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateClubAdvertisement(ClubAdvertisement clubAdvertisement)
        {
            _dbContext.ClubAdvertisements.Update(clubAdvertisement);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteClubAdvertisement(int clubAdvertisementId)
        {
            var clubAdvertisement = await _dbContext.ClubAdvertisements.FindAsync(clubAdvertisementId);
            if (clubAdvertisement == null)
                throw new ArgumentException($"No Club Advertisement found with ID {clubAdvertisementId}");

            if (clubAdvertisement.SalaryRangeId != null)
            {
                var salaryRange = await _dbContext.SalaryRanges.FindAsync(clubAdvertisement.SalaryRangeId);
                if (salaryRange != null)
                    _dbContext.SalaryRanges.Remove(salaryRange);
            }

            var favorites = await _dbContext.FavoriteClubAdvertisements
                .Where(ca => ca.ClubAdvertisementId == clubAdvertisementId)
                .ToListAsync();
            _dbContext.FavoriteClubAdvertisements.RemoveRange(favorites);

            var playerOffers = await _dbContext.PlayerOffers
                .Where(po => po.ClubAdvertisementId == clubAdvertisementId)
                .ToListAsync();
            _dbContext.PlayerOffers.RemoveRange(playerOffers);

            _dbContext.ClubAdvertisements.Remove(clubAdvertisement);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<MemoryStream> ExportClubAdvertisementsToCsv()
        {
            var clubAdvertisements = await GetAllClubAdvertisements();
            var csv = new StringBuilder();
            csv.AppendLine("E-mail,First Name,Last Name,Position,Club Name,League,Region,Min Salary,Max Salary,Creation Date,End Date");

            foreach (var ad in clubAdvertisements)
            {
                csv.AppendLine($"{ad.ClubMember.Email},{ad.ClubMember.FirstName},{ad.ClubMember.LastName},{ad.PlayerPosition.PositionName},{ad.ClubName},{ad.League},{ad.Region},{ad.SalaryRange.Min},{ad.SalaryRange.Max},{ad.CreationDate:yyyy-MM-dd},{ad.EndDate:yyyy-MM-dd}");
            }

            var byteArray = Encoding.UTF8.GetBytes(csv.ToString());
            var csvStream = new MemoryStream(byteArray);

            return csvStream;
        }
    }
}