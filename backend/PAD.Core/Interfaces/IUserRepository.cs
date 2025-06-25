using PAD.Core.Entities;

namespace PAD.Core.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetUserByPrincipalNameAsync(string userPrincipalName);
    Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId);
    Task<User?> GetUserWithRolesAsync(int userId);
} 