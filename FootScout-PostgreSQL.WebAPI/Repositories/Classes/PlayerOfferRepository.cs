using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FootScout_PostgreSQL.WebAPI.Repositories.Classes
{
    public class PlayerOfferRepository : IPlayerOfferRepository
    {
        private readonly AppDbContext _dbContext;

        public PlayerOfferRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PlayerOffer> GetPlayerOffer(int playerOfferId)
        {
            return await _dbContext.PlayerOffers
                .Include(po => po.ClubAdvertisement)
                .Include(ca => ca.ClubAdvertisement.PlayerPosition)
                .Include(ca => ca.ClubAdvertisement.SalaryRange)
                .Include(ca => ca.ClubAdvertisement.ClubMember)
                .Include(po => po.OfferStatus)
                .Include(po => po.PlayerPosition)
                .Include(po => po.PlayerFoot)
                .Include(po => po.Player)
                .FirstOrDefaultAsync(po => po.Id == playerOfferId);
        }

        public async Task<IEnumerable<PlayerOffer>> GetPlayerOffers()
        {
            return await _dbContext.PlayerOffers
                .Include(po => po.ClubAdvertisement)
                .Include(ca => ca.ClubAdvertisement.PlayerPosition)
                .Include(ca => ca.ClubAdvertisement.SalaryRange)
                .Include(ca => ca.ClubAdvertisement.ClubMember)
                .Include(po => po.OfferStatus)
                .Include(po => po.PlayerPosition)
                .Include(po => po.PlayerFoot)
                .Include(po => po.Player)
                .OrderByDescending(po => po.CreationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlayerOffer>> GetActivePlayerOffers()
        {
            return await _dbContext.PlayerOffers
                .Include(po => po.ClubAdvertisement)
                .Include(ca => ca.ClubAdvertisement.PlayerPosition)
                .Include(ca => ca.ClubAdvertisement.SalaryRange)
                .Include(ca => ca.ClubAdvertisement.ClubMember)
                .Include(po => po.OfferStatus)
                .Include(po => po.PlayerPosition)
                .Include(po => po.PlayerFoot)
                .Include(po => po.Player)
                .Where(po => po.ClubAdvertisement.EndDate >= DateTime.UtcNow)
                .OrderByDescending(po => po.CreationDate)
                .ToListAsync();
        }

        public async Task<int> GetActivePlayerOfferCount()
        {
            return await _dbContext.PlayerOffers.Where(po => po.ClubAdvertisement.EndDate >= DateTime.UtcNow).CountAsync();
        }

        public async Task<IEnumerable<PlayerOffer>> GetInactivePlayerOffers()
        {
            return await _dbContext.PlayerOffers
                .Include(po => po.ClubAdvertisement)
                .Include(ca => ca.ClubAdvertisement.PlayerPosition)
                .Include(ca => ca.ClubAdvertisement.SalaryRange)
                .Include(ca => ca.ClubAdvertisement.ClubMember)
                .Include(po => po.OfferStatus)
                .Include(po => po.PlayerPosition)
                .Include(po => po.PlayerFoot)
                .Include(po => po.Player)
                .Where(po => po.ClubAdvertisement.EndDate < DateTime.UtcNow)
                .OrderByDescending(po => po.CreationDate)
                .ToListAsync();
        }

        public async Task CreatePlayerOffer(PlayerOffer playerOffer)
        {
            playerOffer.CreationDate = DateTime.UtcNow;

            var offeredStatus = await _dbContext.OfferStatuses
                .FirstOrDefaultAsync(a => a.StatusName == "Offered");
            playerOffer.OfferStatusId = offeredStatus.Id;
            playerOffer.OfferStatus = offeredStatus;

            await _dbContext.PlayerOffers.AddAsync(playerOffer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdatePlayerOffer(PlayerOffer playerOffer)
        {
            _dbContext.PlayerOffers.Update(playerOffer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePlayerOffer(int playerOfferId)
        {
            var playerOffer = await _dbContext.PlayerOffers.FindAsync(playerOfferId);
            if (playerOffer == null)
            {
                throw new Exception("Player offer not found");
            }

            _dbContext.PlayerOffers.Remove(playerOffer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AcceptPlayerOffer(PlayerOffer playerOffer)
        {
            var acceptedStatus = await _dbContext.OfferStatuses
                .FirstOrDefaultAsync(a => a.StatusName == "Accepted");
            playerOffer.OfferStatusId = acceptedStatus.Id;
            playerOffer.OfferStatus = acceptedStatus;

            _dbContext.PlayerOffers.Update(playerOffer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RejectPlayerOffer(PlayerOffer playerOffer)
        {
            var rejectedStatus = await _dbContext.OfferStatuses
                .FirstOrDefaultAsync(a => a.StatusName == "Rejected");
            playerOffer.OfferStatusId = rejectedStatus.Id;
            playerOffer.OfferStatus = rejectedStatus;

            _dbContext.PlayerOffers.Update(playerOffer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> GetPlayerOfferStatusId(int clubAdvertisementId, string userId)
        {
            var offerStatusId = await _dbContext.PlayerOffers
                .Where(co => co.ClubAdvertisementId == clubAdvertisementId && co.PlayerId == userId)
                .Select(co => co.OfferStatusId)
                .FirstOrDefaultAsync();

            return offerStatusId;
        }

        public async Task<MemoryStream> ExportPlayerOffersToCsv()
        {
            var playerOffers = await GetPlayerOffers();
            var csv = new StringBuilder();
            csv.AppendLine("Offer Status,E-mail,First Name,Last Name,Position,Age,Height,Foot,Salary,Additional Information,Club Member's E-mail,Club Member's First Name,Club Member's Last Name,Club Name,League,Region,Creation Date,End Date");

            foreach (var offer in playerOffers)
            {
                csv.AppendLine($"{offer.OfferStatus.StatusName},{offer.Player.Email},{offer.Player.FirstName},{offer.Player.LastName},{offer.PlayerPosition.PositionName},{offer.Age},{offer.Height},{offer.PlayerFoot.FootName},{offer.Salary},{offer.AdditionalInformation},{offer.ClubAdvertisement.ClubMember.Email},{offer.ClubAdvertisement.ClubMember.FirstName},{offer.ClubAdvertisement.ClubMember.LastName},{offer.ClubAdvertisement.ClubName},{offer.ClubAdvertisement.League},{offer.ClubAdvertisement.Region}{offer.CreationDate:yyyy-MM-dd},{offer.ClubAdvertisement.EndDate:yyyy-MM-dd}");
            }

            var byteArray = Encoding.UTF8.GetBytes(csv.ToString());
            var csvStream = new MemoryStream(byteArray);

            return csvStream;
        }
    }
}