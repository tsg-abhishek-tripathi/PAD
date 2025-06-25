using PAD.Core.Entities;

namespace PAD.Core.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetEmployeeWithDetailsAsync(int id);
    Task<IEnumerable<Employee>> GetEmployeesByOfficeAsync(int officeId);
    Task<IEnumerable<Employee>> GetEmployeesByRoleAsync(int roleId);
    Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm);
} 