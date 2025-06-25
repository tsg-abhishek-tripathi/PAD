namespace PAD.Core.Models;

public class CreateRoleRequest
{
    public int EmployeeId { get; set; }
    public string RoleTypeCode { get; set; } = string.Empty;
    public string? PrimaryPracticeCode { get; set; }
    public string? SecondaryPracticeCode { get; set; }
    public string? LocationCode { get; set; }
    public string? LocationScope { get; set; }
    public DateTime EffectiveDate { get; set; } = DateTime.Today;
    public DateTime? ExpirationDate { get; set; }
    public string ChangeReason { get; set; } = string.Empty;
}

public class UpdateRoleRequest
{
    public string? PrimaryPracticeCode { get; set; }
    public string? SecondaryPracticeCode { get; set; }
    public string? LocationCode { get; set; }
    public string? LocationScope { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public string ChangeReason { get; set; } = string.Empty;
} 