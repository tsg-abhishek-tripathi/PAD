using System.ComponentModel.DataAnnotations;

namespace PAD.Core.Entities;

public class Office
{
    public int OfficeId { get; set; }
    
    [Required]
    [StringLength(10)]
    public string OfficeCode { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string OfficeName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Country { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Region { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Cluster { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime ModifiedDate { get; set; }
    
    // Navigation properties
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public virtual ICollection<EmployeeAffiliation> Affiliations { get; set; } = new List<EmployeeAffiliation>();
    public virtual ICollection<EmployeeRole> Roles { get; set; } = new List<EmployeeRole>();
    
    // Display properties
    public string DisplayName => $"{OfficeName}, {Country}";
    public string FullDisplayName => $"{OfficeName}, {Country} ({Region})";
} 