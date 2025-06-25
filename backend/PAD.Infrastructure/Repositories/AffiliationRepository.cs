using Microsoft.EntityFrameworkCore;
using PAD.Core.Entities;
using PAD.Core.Interfaces;
using PAD.Infrastructure.Data;

namespace PAD.Infrastructure.Repositories;

public class AffiliationRepository : Repository<EmployeeAffiliation>, IAffiliationRepository
{
    public AffiliationRepository(PadDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<EmployeeAffiliation>> GetAffiliationsByEmployeeAsync(int employeeId)
    {
        return await _dbSet
            .Include(a => a.Employee)
            .Include(a => a.Practice)
            .Where(a => a.EmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<EmployeeAffiliation>> GetAffiliationsByPracticeAsync(int practiceId)
    {
        return await _dbSet
            .Include(a => a.Employee)
            .Include(a => a.Practice)
            .Where(a => a.PracticeId == practiceId)
            .ToListAsync();
    }

    public async Task<IEnumerable<EmployeeAffiliation>> GetActiveAffiliationsAsync()
    {
        return await _dbSet
            .Include(a => a.Employee)
            .Include(a => a.Practice)
            .Where(a => a.IsActive)
            .ToListAsync();
    }
} 