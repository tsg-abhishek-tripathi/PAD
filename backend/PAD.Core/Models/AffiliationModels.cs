namespace PAD.Core.Models;

public class BulkAffiliationRequest
{
    public List<int> EmployeeIds { get; set; } = new();
    public string Action { get; set; } = string.Empty; // Add, Update, Remove
    public string RoleTypeCode { get; set; } = string.Empty;
    public string? PracticeCode { get; set; }
    public string? LocationCode { get; set; }
    public string? LocationScope { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string ChangeReason { get; set; } = string.Empty;
}

public class CreateAffiliationRequest
{
    public int EmployeeId { get; set; }
    public string RoleTypeCode { get; set; } = string.Empty;
    public string PracticeCode { get; set; } = string.Empty;
    public string? LocationCode { get; set; }
    public string? LocationScope { get; set; }
    public DateTime EffectiveDate { get; set; } = DateTime.Today;
    public DateTime? ExpirationDate { get; set; }
    public string Source { get; set; } = "Manual";
    public string ChangeReason { get; set; } = string.Empty;
}

public class UpdateAffiliationRequest
{
    public string? LocationCode { get; set; }
    public string? LocationScope { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public string ChangeReason { get; set; } = string.Empty;
}

public class BulkOperationResult
{
    public int TotalRequested { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<BulkOperationError> Errors { get; set; } = new();
    public List<string> SuccessMessages { get; set; } = new();
}

public class BulkOperationError
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
} 