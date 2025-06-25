using System.ComponentModel.DataAnnotations;

namespace PAD.Core.Entities;

public class EmployeeAffiliation
{
    public int AffiliationId { get; set; }
    
    public int EmployeeId { get; set; }
    
    public int RoleTypeId { get; set; }
    
    [Required]
    [StringLength(50)]
    public string RoleTypeCode { get; set; } = string.Empty;
    
    public int PracticeId { get; set; }
    
    [Required]
    [StringLength(50)]
    public string PracticeCode { get; set; } = string.Empty;
    
    public int? LocationId { get; set; }
    
    [StringLength(50)]
    public string? LocationScope { get; set; }
    
    public DateTime EffectiveDate { get; set; }
    
    public DateTime? ExpirationDate { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    [Required]
    [StringLength(50)]
    public string Source { get; set; } = "Manual";
    
    [Required]
    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;
    
    public DateTime CreatedDate { get; set; }
    
    [Required]
    [StringLength(100)]
    public string ModifiedBy { get; set; } = string.Empty;
    
    public DateTime ModifiedDate { get; set; }
    
    // Navigation properties
    public virtual Employee Employee { get; set; } = null!;
    public virtual RoleType RoleType { get; set; } = null!;
    public virtual Taxonomy Practice { get; set; } = null!;
    public virtual Office? Location { get; set; }
    
    // Computed properties
    public bool IsExpired => ExpirationDate.HasValue && ExpirationDate.Value < DateTime.Today;
    public bool IsEffective => EffectiveDate <= DateTime.Today && !IsExpired;
    public string DisplayName => $"{RoleType?.RoleName} - {Practice?.Name}";
} 