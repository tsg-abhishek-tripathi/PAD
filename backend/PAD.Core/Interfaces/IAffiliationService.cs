using PAD.Core.Entities;
using PAD.Core.Models;

namespace PAD.Core.Interfaces;

public interface IAffiliationService
{
    Task<EmployeeAffiliation?> GetAffiliationAsync(int affiliationId);
    Task<IEnumerable<EmployeeAffiliation>> GetEmployeeAffiliationsAsync(int employeeId);
    Task<IEnumerable<EmployeeAffiliation>> GetAffiliationsByPracticeAsync(int practiceId, int userId);
    Task<EmployeeAffiliation> CreateAffiliationAsync(EmployeeAffiliation affiliation);
    Task UpdateAffiliationAsync(EmployeeAffiliation affiliation);
    Task DeleteAffiliationAsync(int affiliationId);
    Task<List<ValidationError>> ValidateAffiliationAsync(int employeeId, string roleTypeCode, string practiceCode);
    Task<bool> CanUserEditAffiliationAsync(int affiliationId, int userId);
    Task BulkUpdateAffiliationsAsync(BulkAffiliationRequest request, int userId);
}

public interface IEmployeeRoleService
{
    Task<List<EmployeeRole>> GetEmployeeRolesAsync(int employeeId, int userId);
    Task<EmployeeRole?> GetRoleByIdAsync(int roleId, int userId);
    Task<EmployeeRole> CreateRoleAsync(CreateRoleRequest request, int userId);
    Task<EmployeeRole> UpdateRoleAsync(int roleId, UpdateRoleRequest request, int userId);
    Task<bool> DeleteRoleAsync(int roleId, string reason, int userId);
    Task<List<ValidationError>> ValidateRoleAsync(int employeeId, string roleTypeCode, int? primaryPracticeId, int? secondaryPracticeId);
    Task<bool> CanUserEditRoleAsync(int roleId, int userId);
}

 