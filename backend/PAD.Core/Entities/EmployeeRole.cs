using System.ComponentModel.DataAnnotations;

namespace PAD.Core.Entities;

public class EmployeeRole
{
    public int EmployeeRoleId { get; set; }
    
    public int EmployeeId { get; set; }
    
    public int RoleTypeId { get; set; }
    
    public int? PrimaryPracticeId { get; set; }
    
    public int? SecondaryPracticeId { get; set; } // For Interlock Champions
    
    public int? LocationId { get; set; }
    
    [StringLength(50)]
    public string? LocationScope { get; set; } // Global, Regional, Local
    
    public DateTime EffectiveDate { get; set; }
    
    public DateTime? ExpirationDate { get; set; }
    
    public bool IsActive { get; set; } = true;
    
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
    public virtual Taxonomy? PrimaryPractice { get; set; }
    public virtual Taxonomy? SecondaryPractice { get; set; }
    public virtual Office? Location { get; set; }
    
    // Computed properties
    public bool IsExpired => ExpirationDate.HasValue && ExpirationDate.Value < DateTime.Today;
    public bool IsEffective => EffectiveDate <= DateTime.Today && !IsExpired;
    public string DisplayName => BuildDisplayName();
    public bool IsInterlockChampion => RoleType?.RoleCode == "INTERLOCK" && SecondaryPracticeId.HasValue;
    
    private string BuildDisplayName()
    {
        var roleName = RoleType?.RoleName ?? "Unknown Role";
        var practices = new List<string>();
        
        if (PrimaryPractice != null)
            practices.Add(PrimaryPractice.Name);
            
        if (SecondaryPractice != null)
            practices.Add(SecondaryPractice.Name);
            
        var practiceText = practices.Any() ? $" - {string.Join(" + ", practices)}" : "";
        var locationText = Location != null ? $" ({Location.OfficeName})" : 
                          !string.IsNullOrEmpty(LocationScope) ? $" ({LocationScope})" : "";
        
        return $"{roleName}{practiceText}{locationText}";
    }
} 