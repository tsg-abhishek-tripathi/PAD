using PAD.Core.Entities;
using PAD.Core.Interfaces;

namespace PAD.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditService _auditService;

    public UserService(IUserRepository userRepository, IAuditService auditService)
    {
        _userRepository = userRepository;
        _auditService = auditService;
    }

    public async Task<User?> GetUserAsync(int userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    public async Task<User?> GetUserByPrincipalNameAsync(string userPrincipalName)
    {
        return await _userRepository.GetUserByPrincipalNameAsync(userPrincipalName);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        var result = await _userRepository.AddAsync(user);
        await _auditService.LogAuditAsync("User", "Create", user.UserId.ToString());
        return result;
    }

    public async Task UpdateUserAsync(User user)
    {
        await _userRepository.UpdateAsync(user);
        await _auditService.LogAuditAsync("User", "Update", user.UserId.ToString());
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user != null)
        {
            await _userRepository.DeleteAsync(user);
            await _auditService.LogAuditAsync("User", "Delete", userId.ToString());
        }
    }
} 