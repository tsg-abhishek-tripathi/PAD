using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PAD.Core.Entities;
using PAD.Core.Interfaces;
using PAD.Infrastructure.Data;
using PAD.Core.Models;

namespace PAD.Infrastructure.Services;

public class EmployeeService : IEmployeeService
{
    private readonly PadDbContext _context;
    private readonly ILogger<EmployeeService> _logger;
    private readonly IAuditService _auditService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(
        PadDbContext context,
        ILogger<EmployeeService> logger,
        IAuditService auditService,
        IAuthorizationService authorizationService,
        IEmployeeRepository employeeRepository)
    {
        _context = context;
        _logger = logger;
        _auditService = auditService;
        _authorizationService = authorizationService;
        _employeeRepository = employeeRepository;
    }

    public async Task<PagedResult<Employee>> GetEmployeesAsync(EmployeeSearchCriteria criteria, int userId)
    {
        try
        {
            var query = _context.Employees
                .Include(e => e.HomeOffice)
                .Include(e => e.Affiliations)
                    .ThenInclude(a => a.Practice)
                .AsQueryable();

            // Apply authorization filters
            var allowedOffices = await _authorizationService.GetUserAllowedOfficesAsync(userId);
            if (allowedOffices.Any())
            {
                query = query.Where(e => allowedOffices.Contains(e.HomeOfficeId));
            }

            // Apply search filters
            if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
            {
                var searchTerm = criteria.SearchTerm.ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(searchTerm) ||
                    e.LastName.ToLower().Contains(searchTerm) ||
                    e.Email.ToLower().Contains(searchTerm) ||
                    e.EmployeeCode.ToLower().Contains(searchTerm));
            }

            if (criteria.OfficeId.HasValue)
            {
                query = query.Where(e => e.HomeOfficeId == criteria.OfficeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(criteria.Region))
            {
                query = query.Where(e => e.HomeOffice != null && e.HomeOffice.Region == criteria.Region);
            }

            if (!string.IsNullOrWhiteSpace(criteria.Level))
            {
                query = query.Where(e => e.Level == criteria.Level);
            }

            if (criteria.IsActive.HasValue)
            {
                query = query.Where(e => e.IsActive == criteria.IsActive.Value);
            }

            if (criteria.PracticeId.HasValue)
            {
                query = query.Where(e => e.Affiliations.Any(a => 
                    a.PracticeId == criteria.PracticeId.Value && a.IsActive));
            }

            // Apply sorting
            query = ApplySorting(query, criteria.SortBy, criteria.SortDescending);

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var employees = await query
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToListAsync();

            return new PagedResult<Employee>
            {
                Items = employees,
                TotalCount = totalCount,
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employees with criteria: {@Criteria}", criteria);
            throw;
        }
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int employeeId, int userId)
    {
        try
        {
            var employee = await _context.Employees
                .Include(e => e.HomeOffice)
                .Include(e => e.Affiliations)
                    .ThenInclude(a => a.Practice)
                .Include(e => e.Affiliations)
                    .ThenInclude(a => a.RoleType)
                .Include(e => e.Roles)
                    .ThenInclude(r => r.RoleType)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null)
                return null;

            // Check authorization
            var canView = await _authorizationService.CanUserViewEmployeeAsync(userId, employeeId);
            if (!canView)
            {
                throw new UnauthorizedAccessException("User does not have permission to view this employee");
            }

            return employee;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employee {EmployeeId}", employeeId);
            throw;
        }
    }

    public async Task<Employee?> GetEmployeeByCodeAsync(string employeeCode, int userId)
    {
        try
        {
            var employee = await _context.Employees
                .Include(e => e.HomeOffice)
                .FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode);

            if (employee == null)
                return null;

            // Check authorization
            var canView = await _authorizationService.CanUserViewEmployeeAsync(userId, employee.EmployeeId);
            if (!canView)
            {
                throw new UnauthorizedAccessException("User does not have permission to view this employee");
            }

            return employee;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employee by code {EmployeeCode}", employeeCode);
            throw;
        }
    }

    public async Task<Employee> CreateEmployeeAsync(CreateEmployeeRequest request, int userId)
    {
        try
        {
            // Validate authorization
            var canCreate = await _authorizationService.CanUserCreateEmployeeAsync(userId);
            if (!canCreate)
            {
                throw new UnauthorizedAccessException("User does not have permission to create employees");
            }

            // Validate request
            await ValidateCreateEmployeeRequest(request);

            var employee = new Employee
            {
                EmployeeCode = request.EmployeeCode,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                HomeOfficeId = request.HomeOfficeId,
                Level = request.Level,
                Title = request.Title,
                HireDate = request.HireDate,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Log audit trail
            await _auditService.LogChangeAsync("Employee", employee.EmployeeId.ToString(), "Status", 
                string.Empty, "Created", userId.ToString());

            _logger.LogInformation("Employee created: {EmployeeId} - {FullName}", 
                employee.EmployeeId, employee.FullName);

            return employee;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee: {@Request}", request);
            throw;
        }
    }

    public async Task<Employee> UpdateEmployeeAsync(int employeeId, UpdateEmployeeRequest request, int userId)
    {
        try
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found");
            }

            // Check authorization
            var canEdit = await _authorizationService.CanUserEditEmployeeAsync(userId, employeeId);
            if (!canEdit)
            {
                throw new UnauthorizedAccessException("User does not have permission to edit this employee");
            }

            // Store original values for audit
            var originalValues = new
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                HomeOfficeId = employee.HomeOfficeId,
                Level = employee.Level,
                Title = employee.Title,
                IsActive = employee.IsActive,
                HireDate = employee.HireDate
            };

            // Update employee properties
            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Email = request.Email;
            employee.HomeOfficeId = request.HomeOfficeId;
            employee.Level = request.Level;
            employee.Title = request.Title;
            employee.IsActive = request.IsActive;
            employee.HireDate = request.HireDate;
            employee.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Log audit trail for each changed property
            if (originalValues.FirstName != employee.FirstName)
                await _auditService.LogChangeAsync("Employee", employeeId.ToString(), "FirstName", originalValues.FirstName ?? string.Empty, employee.FirstName ?? string.Empty, userId.ToString());
            if (originalValues.LastName != employee.LastName)
                await _auditService.LogChangeAsync("Employee", employeeId.ToString(), "LastName", originalValues.LastName ?? string.Empty, employee.LastName ?? string.Empty, userId.ToString());
            if (originalValues.Email != employee.Email)
                await _auditService.LogChangeAsync("Employee", employeeId.ToString(), "Email", originalValues.Email ?? string.Empty, employee.Email ?? string.Empty, userId.ToString());
            if (originalValues.HomeOfficeId != employee.HomeOfficeId)
                await _auditService.LogChangeAsync("Employee", employeeId.ToString(), "HomeOfficeId", originalValues.HomeOfficeId.ToString(), employee.HomeOfficeId.ToString(), userId.ToString());
            if (originalValues.Level != employee.Level)
                await _auditService.LogChangeAsync("Employee", employeeId.ToString(), "Level", originalValues.Level ?? string.Empty, employee.Level ?? string.Empty, userId.ToString());
            if (originalValues.Title != employee.Title)
                await _auditService.LogChangeAsync("Employee", employeeId.ToString(), "Title", originalValues.Title ?? string.Empty, employee.Title ?? string.Empty, userId.ToString());
            if (originalValues.IsActive != employee.IsActive)
                await _auditService.LogChangeAsync("Employee", employeeId.ToString(), "IsActive", originalValues.IsActive.ToString(), employee.IsActive.ToString(), userId.ToString());
            if (originalValues.HireDate != employee.HireDate)
                await _auditService.LogChangeAsync("Employee", employeeId.ToString(), "HireDate", originalValues.HireDate.ToString(), employee.HireDate.ToString(), userId.ToString());

            _logger.LogInformation("Employee updated: {EmployeeId} - {FullName}", 
                employeeId, employee.FullName);

            return employee;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating employee {EmployeeId}: {@Request}", employeeId, request);
            throw;
        }
    }

    public async Task<bool> DeleteEmployeeAsync(int employeeId, int userId)
    {
        try
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null)
                return false;

            // Check authorization
            var canDelete = await _authorizationService.CanUserDeleteEmployeeAsync(userId, employeeId);
            if (!canDelete)
            {
                throw new UnauthorizedAccessException("User does not have permission to delete this employee");
            }

            // Soft delete
            employee.IsActive = false;
            employee.TerminationDate = DateTime.Today;
            employee.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Log audit trail
            await _auditService.LogChangeAsync("Employee", employeeId.ToString(), "Status", 
                "Active", "Deleted", userId.ToString());

            _logger.LogInformation("Employee deleted: {EmployeeId} - {FullName}", 
                employeeId, employee.FullName);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting employee {EmployeeId}", employeeId);
            throw;
        }
    }

    public async Task<bool> CanUserEditEmployeeAsync(int employeeId, int userId)
    {
        return await _authorizationService.CanUserEditEmployeeAsync(userId, employeeId);
    }

    public async Task<List<Employee>> GetEmployeesByOfficeAsync(int officeId, int userId)
    {
        try
        {
            // Check authorization
            var allowedOffices = await _authorizationService.GetUserAllowedOfficesAsync(userId);
            if (allowedOffices.Any() && !allowedOffices.Contains(officeId))
            {
                throw new UnauthorizedAccessException("User does not have permission to view employees from this office");
            }

            return await _context.Employees
                .Include(e => e.HomeOffice)
                .Where(e => e.HomeOfficeId == officeId && e.IsActive)
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employees by office {OfficeId}", officeId);
            throw;
        }
    }

    public async Task<List<Employee>> GetEmployeesByPracticeAsync(int practiceId, int userId)
    {
        try
        {
            return await _context.Employees
                .Include(e => e.HomeOffice)
                .Include(e => e.Affiliations)
                .Where(e => e.IsActive && e.Affiliations.Any(a => 
                    a.PracticeId == practiceId && a.IsActive))
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employees by practice {PracticeId}", practiceId);
            throw;
        }
    }

    public async Task<EmployeeSummary> GetEmployeeSummaryAsync(int employeeId, int userId)
    {
        try
        {
            var employee = await GetEmployeeByIdAsync(employeeId, userId);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found");
            }

            var affiliations = await _context.EmployeeAffiliations
                .Include(a => a.Practice)
                .Include(a => a.RoleType)
                .Include(a => a.Location)
                .Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.IsActive)
                .ThenBy(a => a.EffectiveDate)
                .ToListAsync();

            var roles = await _context.EmployeeRoles
                .Include(r => r.RoleType)
                .Include(r => r.PrimaryPractice)
                .Include(r => r.SecondaryPractice)
                .Include(r => r.Location)
                .Where(r => r.EmployeeId == employeeId)
                .OrderByDescending(r => r.IsActive)
                .ThenBy(r => r.EffectiveDate)
                .ToListAsync();

            return new EmployeeSummary
            {
                Employee = employee,
                Affiliations = affiliations,
                Roles = roles,
                TotalAffiliations = affiliations.Count(a => a.IsActive),
                TotalRoles = roles.Count(r => r.IsActive),
                LastModified = new[] { employee.ModifiedDate }
                    .Concat(affiliations.Select(a => a.ModifiedDate))
                    .Concat(roles.Select(r => r.ModifiedDate))
                    .Max()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employee summary {EmployeeId}", employeeId);
            throw;
        }
    }

    private async Task ValidateCreateEmployeeRequest(CreateEmployeeRequest request)
    {
        var errors = new List<string>();

        // Check if employee code already exists
        var existingEmployee = await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeCode == request.EmployeeCode);
        if (existingEmployee != null)
        {
            errors.Add($"Employee with code '{request.EmployeeCode}' already exists");
        }

        // Check if email already exists
        existingEmployee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Email == request.Email);
        if (existingEmployee != null)
        {
            errors.Add($"Employee with email '{request.Email}' already exists");
        }

        // Validate office exists
        var office = await _context.Offices
            .FirstOrDefaultAsync(o => o.OfficeId == request.HomeOfficeId && o.IsActive);
        if (office == null)
        {
            errors.Add($"Office with ID {request.HomeOfficeId} not found or inactive");
        }

        if (errors.Any())
        {
            throw new ArgumentException(string.Join(", ", errors));
        }
    }

    private static IQueryable<Employee> ApplySorting(IQueryable<Employee> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLower() switch
        {
            "firstname" => sortDescending ? query.OrderByDescending(e => e.FirstName) : query.OrderBy(e => e.FirstName),
            "lastname" => sortDescending ? query.OrderByDescending(e => e.LastName) : query.OrderBy(e => e.LastName),
            "email" => sortDescending ? query.OrderByDescending(e => e.Email) : query.OrderBy(e => e.Email),
            "level" => sortDescending ? query.OrderByDescending(e => e.Level) : query.OrderBy(e => e.Level),
            "hiredate" => sortDescending ? query.OrderByDescending(e => e.HireDate) : query.OrderBy(e => e.HireDate),
            "office" => sortDescending ? query.OrderByDescending(e => e.HomeOffice.OfficeName) : query.OrderBy(e => e.HomeOffice.OfficeName),
            _ => query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
        };
    }

    public async Task<Employee?> GetEmployeeAsync(int employeeId)
    {
        return await _context.Employees.FindAsync(employeeId);
    }

    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        return await _context.Employees.ToListAsync();
    }

    public async Task<Employee> CreateEmployeeAsync(Employee employee)
    {
        employee.CreatedDate = DateTime.UtcNow;
        employee.ModifiedDate = DateTime.UtcNow;
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task UpdateEmployeeAsync(Employee employee)
    {
        employee.ModifiedDate = DateTime.UtcNow;
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ActivateEmployeeAsync(int employeeId, int userId)
    {
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee == null) return false;

        var oldStatus = employee.Status;
        const string newStatus = "Active";
        employee.Status = newStatus;
        employee.ModifiedDate = DateTime.UtcNow;
        employee.ModifiedBy = userId.ToString();

        await _context.SaveChangesAsync();
        await _auditService.LogChangeAsync(
            entityType: "Employee",
            entityId: employeeId.ToString(),
            field: "Status",
            oldValue: oldStatus ?? "Unknown",
            newValue: newStatus,
            userId: userId.ToString());

        return true;
    }

    public async Task<bool> DeactivateEmployeeAsync(int employeeId, int userId)
    {
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee == null) return false;

        var oldStatus = employee.Status;
        const string newStatus = "Inactive";
        employee.Status = newStatus;
        employee.ModifiedDate = DateTime.UtcNow;
        employee.ModifiedBy = userId.ToString();

        await _context.SaveChangesAsync();
        await _auditService.LogChangeAsync(
            entityType: "Employee",
            entityId: employeeId.ToString(),
            field: "Status",
            oldValue: oldStatus ?? "Unknown",
            newValue: newStatus,
            userId: userId.ToString());

        return true;
    }

    public async Task<bool> TerminateEmployeeAsync(int employeeId, int userId)
    {
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee == null) return false;

        var oldStatus = employee.Status;
        const string newStatus = "Terminated";
        employee.Status = newStatus;
        employee.ModifiedDate = DateTime.UtcNow;
        employee.ModifiedBy = userId.ToString();

        await _context.SaveChangesAsync();
        await _auditService.LogChangeAsync(
            entityType: "Employee",
            entityId: employeeId.ToString(),
            field: "Status",
            oldValue: oldStatus ?? "Unknown",
            newValue: newStatus,
            userId: userId.ToString());

        return true;
    }
} 