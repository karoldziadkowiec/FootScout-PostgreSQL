using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FootScout_PostgreSQL.WebAPI.Repositories.Classes
{
    public class ClubOfferRepository : IClubOfferRepository
    {
        private readonly AppDbContext _dbContext;

        public ClubOfferRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ClubOffer> GetClubOffer(int clubOfferId)
        {
            return await _dbContext.ClubOffers
                .Include(co => co.PlayerAdvertisement)
                .Include(pa => pa.PlayerAdvertisement.PlayerPosition)
                .Include(pa => pa.PlayerAdvertisement.PlayerFoot)
                .Include(pa => pa.PlayerAdvertisement.SalaryRange)
                .Include(pa => pa.PlayerAdvertisement.Player)
                .Include(co => co.OfferStatus)
                .Include(co => co.PlayerPosition)
                .Include(co => co.ClubMember)
                .FirstOrDefaultAsync(co => co.Id == clubOfferId);
        }

        public async Task<IEnumerable<ClubOffer>> GetClubOffers()
        {
            return await _dbContext.ClubOffers
                .Include(co => co.PlayerAdvertisement)
                .Include(pa => pa.PlayerAdvertisement.PlayerPosition)
                .Include(pa => pa.PlayerAdvertisement.PlayerFoot)
                .Include(pa => pa.PlayerAdvertisement.SalaryRange)
                .Include(pa => pa.PlayerAdvertisement.Player)
                .Include(co => co.OfferStatus)
                .Include(co => co.PlayerPosition)
                .Include(co => co.ClubMember)
                .OrderByDescending(co => co.CreationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClubOffer>> GetActiveClubOffers()
        {
            return await _dbContext.ClubOffers
                .Include(co => co.PlayerAdvertisement)
                .Include(pa => pa.PlayerAdvertisement.PlayerPosition)
                .Include(pa => pa.PlayerAdvertisement.PlayerFoot)
                .Include(pa => pa.PlayerAdvertisement.SalaryRange)
                .Include(pa => pa.PlayerAdvertisement.Player)
                .Include(co => co.OfferStatus)
                .Include(co => co.PlayerPosition)
                .Include(co => co.ClubMember)
                .Where(co => co.PlayerAdvertisement.EndDate >= DateTime.UtcNow)
                .OrderByDescending(co => co.CreationDate)
                .ToListAsync();
        }

        public async Task<int> GetActiveClubOfferCount()
        {
            return await _dbContext.ClubOffers.Where(co => co.PlayerAdvertisement.EndDate >= DateTime.UtcNow).CountAsync();
        }

        public async Task<IEnumerable<ClubOffer>> GetInactiveClubOffers()
        {
            return await _dbContext.ClubOffers
                .Include(co => co.PlayerAdvertisement)
                .Include(pa => pa.PlayerAdvertisement.PlayerPosition)
                .Include(pa => pa.PlayerAdvertisement.PlayerFoot)
                .Include(pa => pa.PlayerAdvertisement.SalaryRange)
                .Include(pa => pa.PlayerAdvertisement.Player)
                .Include(co => co.OfferStatus)
                .Include(co => co.PlayerPosition)
                .Include(co => co.ClubMember)
                .Where(co => co.PlayerAdvertisement.EndDate < DateTime.UtcNow)
                .OrderByDescending(co => co.CreationDate)
                .ToListAsync();
        }

        public async Task CreateClubOffer(ClubOffer clubOffer)
        {
            clubOffer.CreationDate = DateTime.UtcNow;

            var offeredStatus = await _dbContext.OfferStatuses
                .FirstOrDefaultAsync(a => a.StatusName == "Offered");
            clubOffer.OfferStatusId = offeredStatus.Id;
            clubOffer.OfferStatus = offeredStatus;

            await _dbContext.ClubOffers.AddAsync(clubOffer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateClubOffer(ClubOffer clubOffer)
        {
            _dbContext.ClubOffers.Update(clubOffer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteClubOffer(int clubOfferId)
        {
            var clubOffer = await _dbContext.ClubOffers.FindAsync(clubOfferId);
            if (clubOffer == null)
            {
                throw new Exception("Club offer not found");
            }

            _dbContext.ClubOffers.Remove(clubOffer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AcceptClubOffer(ClubOffer clubOffer)
        {
            var acceptedStatus = await _dbContext.OfferStatuses
                .FirstOrDefaultAsync(a => a.StatusName == "Accepted");
            clubOffer.OfferStatusId = acceptedStatus.Id;
            clubOffer.OfferStatus = acceptedStatus;

            _dbContext.ClubOffers.Update(clubOffer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RejectClubOffer(ClubOffer clubOffer)
        {
            var rejectedStatus = await _dbContext.OfferStatuses
                .FirstOrDefaultAsync(a => a.StatusName == "Rejected");
            clubOffer.OfferStatusId = rejectedStatus.Id;
            clubOffer.OfferStatus = rejectedStatus;

            _dbContext.ClubOffers.Update(clubOffer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> GetClubOfferStatusId(int playerAdvertisementId, string userId)
        {
            var offerStatusId = await _dbContext.ClubOffers
                .Where(co => co.PlayerAdvertisementId == playerAdvertisementId && co.ClubMemberId == userId)
                .Select(co => co.OfferStatusId)
                .FirstOrDefaultAsync();

            return offerStatusId;
        }

        public async Task<MemoryStream> ExportClubOffersToCsv()
        {
            var clubOffers = await GetClubOffers();
            var csv = new StringBuilder();
            csv.AppendLine("Offer Status,E-mail,First Name,Last Name,Position,Club Name,League,Region,Salary,Additional Information,Player's E-mail,Player's First Name,Player's Last Name,Age,Height,Foot,Creation Date,End Date");

            foreach (var offer in clubOffers)
            {
                csv.AppendLine($"{offer.OfferStatus.StatusName},{offer.ClubMember.Email},{offer.ClubMember.FirstName},{offer.ClubMember.LastName},{offer.PlayerPosition.PositionName},{offer.ClubName},{offer.League},{offer.Region},{offer.Salary},{offer.AdditionalInformation},{offer.PlayerAdvertisement.Player.Email},{offer.PlayerAdvertisement.Player.FirstName},{offer.PlayerAdvertisement.Player.LastName},{offer.PlayerAdvertisement.Age},{offer.PlayerAdvertisement.Height},{offer.PlayerAdvertisement.PlayerFoot.FootName}{offer.CreationDate:yyyy-MM-dd},{offer.PlayerAdvertisement.EndDate:yyyy-MM-dd}");
            }

            var byteArray = Encoding.UTF8.GetBytes(csv.ToString());
            var csvStream = new MemoryStream(byteArray);

            return csvStream;
        }
    }
}