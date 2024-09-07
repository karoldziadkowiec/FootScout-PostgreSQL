using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootScout_PostgreSQL.WebAPI.Repositories.Classes
{
    public class OfferStatusRepository : IOfferStatusRepository
    {
        private readonly AppDbContext _dbContext;

        public OfferStatusRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<OfferStatus>> GetOfferStatuses()
            => await _dbContext.OfferStatuses.ToListAsync();

        public async Task<OfferStatus> GetOfferStatus(int offerStatusId)
        {
            return await _dbContext.OfferStatuses.FindAsync(offerStatusId);
        }

        public async Task<string> GetOfferStatusName(int statusId)
            => await _dbContext.OfferStatuses.Where(a => a.Id == statusId).Select(a => a.StatusName).FirstOrDefaultAsync();

        public async Task<int> GetOfferStatusId(string statusName)
            => await _dbContext.OfferStatuses.Where(a => a.StatusName == statusName).Select(a => a.Id).FirstOrDefaultAsync();
    }
}