using Microsoft.EntityFrameworkCore;
using PAD.Core.Entities;
using PAD.Core.Interfaces;
using PAD.Infrastructure.Data;

namespace PAD.Infrastructure.Repositories;

public class OfficeRepository : Repository<Office>, IOfficeRepository
{
    public OfficeRepository(PadDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Office>> GetOfficesByRegionAsync(string region)
    {
        return await _dbSet
            .Where(o => o.Region == region)
            .ToListAsync();
    }

    public async Task<IEnumerable<Office>> GetActiveOfficesAsync()
    {
        return await _dbSet
            .Where(o => o.IsActive)
            .ToListAsync();
    }

    public async Task<Office?> GetOfficeWithEmployeesAsync(int officeId)
    {
        return await _dbSet
            .Include(o => o.Employees)
            .FirstOrDefaultAsync(o => o.OfficeId == officeId);
    }
} 