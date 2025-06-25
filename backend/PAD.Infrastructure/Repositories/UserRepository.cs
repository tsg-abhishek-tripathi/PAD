using Microsoft.EntityFrameworkCore;
using PAD.Core.Entities;
using PAD.Core.Interfaces;
using PAD.Infrastructure.Data;

namespace PAD.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(PadDbContext context) : base(context)
    {
    }

    public async Task<User?> GetUserByPrincipalNameAsync(string userPrincipalName)
    {
        return await _dbSet
            .Include(u => u.UserRoleAssignments)
                .ThenInclude(ura => ura.UserRole)
            .FirstOrDefaultAsync(u => u.UserPrincipalName == userPrincipalName);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId)
    {
        return await _dbSet
            .Include(u => u.UserRoleAssignments)
                .ThenInclude(ura => ura.UserRole)
            .Where(u => u.UserRoleAssignments.Any(ura => ura.UserRoleId == roleId))
            .ToListAsync();
    }

    public async Task<User?> GetUserWithRolesAsync(int userId)
    {
        return await _dbSet
            .Include(u => u.UserRoleAssignments)
                .ThenInclude(ura => ura.UserRole)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }
} 