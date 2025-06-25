using PAD.Core.Entities;

namespace PAD.Core.Interfaces;

public interface IUserService
{
    Task<User?> GetUserAsync(int userId);
    Task<User?> GetUserByPrincipalNameAsync(string userPrincipalName);
    Task<User> CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int userId);
} 