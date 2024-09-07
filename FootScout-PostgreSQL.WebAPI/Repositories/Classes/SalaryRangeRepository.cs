using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Repositories.Interfaces;

namespace FootScout_PostgreSQL.WebAPI.Repositories.Classes
{
    public class SalaryRangeRepository : ISalaryRangeRepository
    {
        private readonly AppDbContext _dbContext;

        public SalaryRangeRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateSalaryRange(SalaryRange salaryRange)
        {
            await _dbContext.SalaryRanges.AddAsync(salaryRange);
            await _dbContext.SaveChangesAsync();
        }
    }
}