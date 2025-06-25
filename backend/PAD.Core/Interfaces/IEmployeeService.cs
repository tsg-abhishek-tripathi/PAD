using PAD.Core.Entities;

namespace PAD.Core.Interfaces;

public interface IEmployeeService
{
    Task<PagedResult<Employee>> GetEmployeesAsync(EmployeeSearchCriteria criteria, int userId);
    Task<Employee?> GetEmployeeByIdAsync(int employeeId, int userId);
    Task<Employee?> GetEmployeeByCodeAsync(string employeeCode, int userId);
    Task<Employee> CreateEmployeeAsync(CreateEmployeeRequest request, int userId);
    Task<Employee> UpdateEmployeeAsync(int employeeId, UpdateEmployeeRequest request, int userId);
    Task<bool> DeleteEmployeeAsync(int employeeId, int userId);
    Task<bool> CanUserEditEmployeeAsync(int employeeId, int userId);
    Task<List<Employee>> GetEmployeesByOfficeAsync(int officeId, int userId);
    Task<List<Employee>> GetEmployeesByPracticeAsync(int practiceId, int userId);
    Task<EmployeeSummary> GetEmployeeSummaryAsync(int employeeId, int userId);
}

public class EmployeeSearchCriteria
{
    public string? SearchTerm { get; set; }
    public int? OfficeId { get; set; }
    public int? PracticeId { get; set; }
    public string? Region { get; set; }
    public string? Level { get; set; }
    public bool? IsActive { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}

public class CreateEmployeeRequest
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int HomeOfficeId { get; set; }
    public string Level { get; set; } = string.Empty;
    public string? Title { get; set; }
    public DateTime? HireDate { get; set; }
}

public class UpdateEmployeeRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int HomeOfficeId { get; set; }
    public string Level { get; set; } = string.Empty;
    public string? Title { get; set; }
    public bool IsActive { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
}

public class EmployeeSummary
{
    public Employee Employee { get; set; } = null!;
    public List<EmployeeAffiliation> Affiliations { get; set; } = new();
    public List<EmployeeRole> Roles { get; set; } = new();
    public int TotalAffiliations { get; set; }
    public int TotalRoles { get; set; }
    public DateTime LastModified { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
} 