using PAD.Core.Entities;

namespace PAD.Core.Interfaces;

public interface IAffiliationRepository : IRepository<EmployeeAffiliation>
{
    Task<IEnumerable<EmployeeAffiliation>> GetAffiliationsByEmployeeAsync(int employeeId);
    Task<IEnumerable<EmployeeAffiliation>> GetAffiliationsByPracticeAsync(int practiceId);
    Task<IEnumerable<EmployeeAffiliation>> GetActiveAffiliationsAsync();
} 