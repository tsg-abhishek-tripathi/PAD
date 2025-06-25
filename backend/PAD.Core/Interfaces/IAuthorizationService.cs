namespace PAD.Core.Interfaces;

public interface IAuthorizationService
{
    Task<bool> CanUserViewEmployeeAsync(int userId, int employeeId);
    Task<bool> CanUserEditEmployeeAsync(int userId, int employeeId);
    Task<bool> CanUserCreateEmployeeAsync(int userId);
    Task<bool> CanUserDeleteEmployeeAsync(int userId, int employeeId);
    Task<bool> CanUserViewAffiliationAsync(int userId, int affiliationId);
    Task<bool> CanUserEditAffiliationAsync(int userId, int affiliationId);
    Task<bool> CanUserCreateAffiliationAsync(int userId, int employeeId);
    Task<bool> CanUserDeleteAffiliationAsync(int userId, int affiliationId);
    Task<bool> CanUserViewRoleAsync(int userId, int roleId);
    Task<bool> CanUserEditRoleAsync(int userId, int roleId);
    Task<bool> CanUserCreateRoleAsync(int userId, int employeeId);
    Task<bool> CanUserDeleteRoleAsync(int userId, int roleId);
    Task<bool> CanUserViewReportsAsync(int userId);
    Task<bool> CanUserManageSystemSettingsAsync(int userId);
    Task<bool> CanUserPerformBulkOperationsAsync(int userId);
    Task<List<int>> GetUserAllowedOfficesAsync(int userId);
    Task<List<int>> GetUserAllowedPracticesAsync(int userId);
    Task<List<string>> GetUserAllowedRegionsAsync(int userId);
    Task<UserPermissions> GetUserPermissionsAsync(int userId);
}

public class UserPermissions
{
    public bool CanViewAllEmployees { get; set; }
    public bool CanEditAllEmployees { get; set; }
    public bool CanCreateEmployees { get; set; }
    public bool CanDeleteEmployees { get; set; }
    public bool CanViewReports { get; set; }
    public bool CanManageSystemSettings { get; set; }
    public bool CanManageBulkOperations { get; set; }
    public List<int> AllowedOffices { get; set; } = new();
    public List<int> AllowedPractices { get; set; } = new();
    public List<string> AllowedRegions { get; set; } = new();
    public List<string> UserRoles { get; set; } = new();
} 