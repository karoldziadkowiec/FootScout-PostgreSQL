using FootScout_PostgreSQL.WebAPI.Entities;

namespace FootScout_PostgreSQL.WebAPI.Repositories.Interfaces
{
    public interface ISalaryRangeRepository
    {
        Task CreateSalaryRange(SalaryRange salaryRange);
    }
}