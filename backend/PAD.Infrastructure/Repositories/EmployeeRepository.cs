using Microsoft.EntityFrameworkCore;
using PAD.Core.Entities;
using PAD.Core.Interfaces;
using PAD.Infrastructure.Data;

namespace PAD.Infrastructure.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(PadDbContext context) : base(context)
    {
    }

    public async Task<Employee?> GetEmployeeWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(e => e.HomeOffice)
            .Include(e => e.Affiliations)
                .ThenInclude(a => a.Practice)
            .Include(e => e.Roles)
                .ThenInclude(r => r.RoleType)
            .FirstOrDefaultAsync(e => e.EmployeeId == id);
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByOfficeAsync(int officeId)
    {
        return await _dbSet
            .Include(e => e.HomeOffice)
            .Where(e => e.HomeOfficeId == officeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByRoleAsync(int roleId)
    {
        return await _dbSet
            .Include(e => e.Roles)
            .Where(e => e.Roles.Any(r => r.RoleTypeId == roleId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm)
    {
        return await _dbSet
            .Include(e => e.HomeOffice)
            .Where(e => e.FirstName.Contains(searchTerm) || 
                       e.LastName.Contains(searchTerm) || 
                       e.Email.Contains(searchTerm))
            .ToListAsync();
    }
} 