using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAD.Core.Interfaces;
using PAD.Core.Entities;
using System.Security.Claims;

namespace PAD.Api.Controllers;

/// <summary>
/// Manages employee-related operations including CRUD operations, office assignments, and practice affiliations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IAuditService _auditService;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(
        IEmployeeService employeeService,
        IAuditService auditService,
        ILogger<EmployeesController> logger)
    {
        _employeeService = employeeService;
        _auditService = auditService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a paginated list of employees based on search criteria
    /// </summary>
    /// <param name="criteria">Search criteria including filters, pagination, and sorting options</param>
    /// <returns>A paginated list of employees matching the criteria</returns>
    /// <response code="200">Returns the list of employees</response>
    /// <response code="400">If the criteria is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EmployeeDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<PagedResult<EmployeeDto>>> GetEmployees([FromQuery] EmployeeSearchCriteria criteria)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _employeeService.GetEmployeesAsync(criteria, userId);
            
            await _auditService.LogAuditAsync("Employee", "Search", criteria.ToString() ?? string.Empty,
                $"Searched employees with criteria: {System.Text.Json.JsonSerializer.Serialize(criteria)}");

            var employeeDtos = result.Items.Select(MapToDto).ToList();
            
            return Ok(new PagedResult<EmployeeDto>
            {
                Items = employeeDtos,
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employees");
            return StatusCode(500, "An error occurred while retrieving employees");
        }
    }

    /// <summary>
    /// Retrieves detailed information about a specific employee
    /// </summary>
    /// <param name="id">The unique identifier of the employee</param>
    /// <returns>Detailed information about the employee including affiliations and roles</returns>
    /// <response code="200">Returns the employee details</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have permission to view this employee</response>
    /// <response code="404">If the employee is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EmployeeDetailDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<EmployeeDetailDto>> GetEmployee(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var employee = await _employeeService.GetEmployeeByIdAsync(id, userId);
            
            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found");
            }

            var summary = await _employeeService.GetEmployeeSummaryAsync(id, userId);
            
            await _auditService.LogAuditAsync("Employee", "View", id.ToString(),
                $"Viewed employee details for {employee.FullName}");

            return Ok(MapToDetailDto(summary));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid("You don't have permission to view this employee");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employee {EmployeeId}", id);
            return StatusCode(500, "An error occurred while retrieving the employee");
        }
    }

    /// <summary>
    /// Creates a new employee record
    /// </summary>
    /// <param name="request">The employee information to create</param>
    /// <returns>The newly created employee</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/employees
    ///     {
    ///         "employeeCode": "E12345",
    ///         "firstName": "John",
    ///         "lastName": "Doe",
    ///         "email": "john.doe@example.com",
    ///         "level": "Manager",
    ///         "title": "Senior Consultant",
    ///         "homeOfficeId": 1,
    ///         "hireDate": "2023-01-01"
    ///     }
    /// </remarks>
    /// <response code="201">Returns the newly created employee</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to create employees</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost]
    [Authorize(Policy = "SystemAdmin")]
    [ProducesResponseType(typeof(EmployeeDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var employee = await _employeeService.CreateEmployeeAsync(request, userId);
            
            await _auditService.LogAuditAsync("Employee", "Create", employee.EmployeeId.ToString(),
                $"Created new employee: {employee.FullName}");

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, MapToDto(employee));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee");
            return StatusCode(500, "An error occurred while creating the employee");
        }
    }

    /// <summary>
    /// Updates an existing employee record
    /// </summary>
    /// <param name="id">The unique identifier of the employee to update</param>
    /// <param name="request">The updated employee information</param>
    /// <returns>The updated employee</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT /api/employees/1
    ///     {
    ///         "firstName": "John",
    ///         "lastName": "Doe",
    ///         "email": "john.doe@example.com",
    ///         "level": "Manager",
    ///         "title": "Senior Consultant",
    ///         "homeOfficeId": 1
    ///     }
    /// </remarks>
    /// <response code="200">Returns the updated employee</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to update employees</response>
    /// <response code="404">If the employee is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "SystemAdmin")]
    [ProducesResponseType(typeof(EmployeeDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<EmployeeDto>> UpdateEmployee(int id, [FromBody] UpdateEmployeeRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var employee = await _employeeService.UpdateEmployeeAsync(id, request, userId);
            
            await _auditService.LogAuditAsync("Employee", "Update", id.ToString(),
                $"Updated employee: {employee.FullName}");

            return Ok(MapToDto(employee));
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Employee with ID {id} not found");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid("You don't have permission to update this employee");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating employee {EmployeeId}", id);
            return StatusCode(500, "An error occurred while updating the employee");
        }
    }

    /// <summary>
    /// Soft deletes an employee record
    /// </summary>
    /// <param name="id">The unique identifier of the employee to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the employee was successfully deleted</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to delete employees</response>
    /// <response code="404">If the employee is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "SystemAdmin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> DeleteEmployee(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _employeeService.DeleteEmployeeAsync(id, userId);
            
            if (!success)
            {
                return NotFound($"Employee with ID {id} not found");
            }

            await _auditService.LogAuditAsync("Employee", "Delete", id.ToString(),
                $"Deleted employee with ID {id}");

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid("You don't have permission to delete this employee");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting employee {EmployeeId}", id);
            return StatusCode(500, "An error occurred while deleting the employee");
        }
    }

    /// <summary>
    /// Retrieves all employees assigned to a specific office
    /// </summary>
    /// <param name="officeId">The unique identifier of the office</param>
    /// <returns>A list of employees in the specified office</returns>
    /// <response code="200">Returns the list of employees</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the office is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("by-office/{officeId}")]
    [ProducesResponseType(typeof(List<EmployeeDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<EmployeeDto>>> GetEmployeesByOffice(int officeId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var employees = await _employeeService.GetEmployeesByOfficeAsync(officeId, userId);
            
            return Ok(employees.Select(MapToDto).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employees by office {OfficeId}", officeId);
            return StatusCode(500, "An error occurred while retrieving employees");
        }
    }

    /// <summary>
    /// Get employees by practice
    /// </summary>
    [HttpGet("by-practice/{practiceId}")]
    public async Task<ActionResult<List<EmployeeDto>>> GetEmployeesByPractice(int practiceId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var employees = await _employeeService.GetEmployeesByPracticeAsync(practiceId, userId);
            
            return Ok(employees.Select(MapToDto).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employees by practice {PracticeId}", practiceId);
            return StatusCode(500, "An error occurred while retrieving employees");
        }
    }

    /// <summary>
    /// Check if user can edit employee
    /// </summary>
    [HttpGet("{id}/can-edit")]
    public async Task<ActionResult<bool>> CanEditEmployee(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var canEdit = await _employeeService.CanUserEditEmployeeAsync(id, userId);
            
            return Ok(canEdit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking edit permissions for employee {EmployeeId}", id);
            return StatusCode(500, "An error occurred while checking permissions");
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("userId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        return userId;
    }

    private static EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            EmployeeId = employee.EmployeeId,
            EmployeeCode = employee.EmployeeCode,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            FullName = employee.FullName,
            Email = employee.Email,
            Level = employee.Level,
            Title = employee.Title,
            IsActive = employee.IsActive,
            HireDate = employee.HireDate,
            TerminationDate = employee.TerminationDate,
            HomeOffice = employee.HomeOffice != null ? new OfficeDto
            {
                OfficeId = employee.HomeOffice.OfficeId,
                OfficeCode = employee.HomeOffice.OfficeCode,
                OfficeName = employee.HomeOffice.OfficeName,
                Country = employee.HomeOffice.Country,
                Region = employee.HomeOffice.Region
            } : null
        };
    }

    private static EmployeeDetailDto MapToDetailDto(EmployeeSummary summary)
    {
        return new EmployeeDetailDto
        {
            Employee = MapToDto(summary.Employee),
            Affiliations = summary.Affiliations.Select(a => new AffiliationDto
            {
                AffiliationId = a.AffiliationId,
                RoleType = a.RoleType?.RoleName ?? "",
                Practice = a.Practice?.Name ?? "",
                LocationScope = a.LocationScope,
                EffectiveDate = a.EffectiveDate,
                ExpirationDate = a.ExpirationDate,
                IsActive = a.IsActive
            }).ToList(),
            Roles = summary.Roles.Select(r => new RoleDto
            {
                EmployeeRoleId = r.EmployeeRoleId,
                RoleType = r.RoleType?.RoleName ?? "",
                PrimaryPractice = r.PrimaryPractice?.Name,
                SecondaryPractice = r.SecondaryPractice?.Name,
                LocationScope = r.LocationScope,
                EffectiveDate = r.EffectiveDate,
                ExpirationDate = r.ExpirationDate,
                IsActive = r.IsActive
            }).ToList(),
            TotalAffiliations = summary.TotalAffiliations,
            TotalRoles = summary.TotalRoles,
            LastModified = summary.LastModified
        };
    }
}

// DTOs for API responses
public class EmployeeDto
{
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string? Title { get; set; }
    public bool IsActive { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public OfficeDto? HomeOffice { get; set; }
}

public class EmployeeDetailDto
{
    public EmployeeDto Employee { get; set; } = null!;
    public List<AffiliationDto> Affiliations { get; set; } = new();
    public List<RoleDto> Roles { get; set; } = new();
    public int TotalAffiliations { get; set; }
    public int TotalRoles { get; set; }
    public DateTime LastModified { get; set; }
}

public class OfficeDto
{
    public int OfficeId { get; set; }
    public string OfficeCode { get; set; } = string.Empty;
    public string OfficeName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}

public class AffiliationDto
{
    public int AffiliationId { get; set; }
    public string RoleType { get; set; } = string.Empty;
    public string Practice { get; set; } = string.Empty;
    public string? LocationScope { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; }
}

public class RoleDto
{
    public int EmployeeRoleId { get; set; }
    public string RoleType { get; set; } = string.Empty;
    public string? PrimaryPractice { get; set; }
    public string? SecondaryPractice { get; set; }
    public string? LocationScope { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; }
} 