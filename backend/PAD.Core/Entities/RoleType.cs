using System.ComponentModel.DataAnnotations;

namespace PAD.Core.Entities;

public class RoleType
{
    public int RoleTypeId { get; set; }
    
    [Required]
    [StringLength(50)]
    public string RoleCategory { get; set; } = string.Empty; // Affiliation, Leadership, Expertise, Access
    
    [Required]
    [StringLength(100)]
    public string RoleName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string RoleCode { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public bool RequiresPracticeSelection { get; set; } = false;
    
    public bool RequiresLocationSelection { get; set; } = false;
    
    public bool AllowsMultiplePerPractice { get; set; } = true;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime ModifiedDate { get; set; }
    
    // Navigation properties
    public virtual ICollection<EmployeeAffiliation> Affiliations { get; set; } = new List<EmployeeAffiliation>();
    public virtual ICollection<EmployeeRole> Roles { get; set; } = new List<EmployeeRole>();
    
    // Computed properties
    public string DisplayName => $"{RoleName} ({RoleCode})";
    public bool IsAffiliationRole => RoleCategory.Equals("Affiliation", StringComparison.OrdinalIgnoreCase);
    public bool IsLeadershipRole => RoleCategory.Equals("Leadership", StringComparison.OrdinalIgnoreCase);
    public bool IsExpertiseRole => RoleCategory.Equals("Expertise", StringComparison.OrdinalIgnoreCase);
    public bool IsAccessRole => RoleCategory.Equals("Access", StringComparison.OrdinalIgnoreCase);
} 