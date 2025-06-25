using System.ComponentModel.DataAnnotations;

namespace PAD.Core.Entities;

public class Taxonomy
{
    public int TaxonomyId { get; set; }
    
    [Required]
    [StringLength(20)]
    public string FacetType { get; set; } = string.Empty; // Industry, Capability, Keyword
    
    public int Level { get; set; } // 1, 2, 3 (hierarchy level)
    
    public int? ParentId { get; set; }
    
    [Required]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public string CreatedBy { get; set; } = string.Empty;
    
    public string ModifiedBy { get; set; } = string.Empty;
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime ModifiedDate { get; set; }
    
    // Navigation properties
    public virtual Taxonomy? Parent { get; set; }
    public virtual ICollection<Taxonomy> Children { get; set; } = new List<Taxonomy>();
    public virtual ICollection<EmployeeAffiliation> Affiliations { get; set; } = new List<EmployeeAffiliation>();
    public virtual ICollection<EmployeeRole> PrimaryRoles { get; set; } = new List<EmployeeRole>();
    public virtual ICollection<EmployeeRole> SecondaryRoles { get; set; } = new List<EmployeeRole>();
    
    // Computed properties
    public string DisplayName => $"{Name} ({Code})";
    public string FullPath => GetFullPath();
    public bool IsTopLevel => Level == 1;
    
    private string GetFullPath()
    {
        var path = Name;
        var current = Parent;
        while (current != null)
        {
            path = $"{current.Name} > {path}";
            current = current.Parent;
        }
        return path;
    }
} 