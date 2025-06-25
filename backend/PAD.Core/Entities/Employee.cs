using System.ComponentModel.DataAnnotations;

namespace PAD.Core.Entities;

public class Employee
{
    public int EmployeeId { get; set; }
    
    [Required]
    [StringLength(20)]
    public string EmployeeCode { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;
    
    public int HomeOfficeId { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Level { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string? Title { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime? HireDate { get; set; }
    
    public DateTime? TerminationDate { get; set; }
    
    public DateTime LastSyncDate { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime ModifiedDate { get; set; }
    
    public string EmployeeNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string CreatedBy { get; set; } = string.Empty;
    public string ModifiedBy { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual Office? HomeOffice { get; set; }
    public virtual ICollection<EmployeeAffiliation> Affiliations { get; set; } = new List<EmployeeAffiliation>();
    public virtual ICollection<EmployeeRole> Roles { get; set; } = new List<EmployeeRole>();
    
    // Computed properties
    public string FullName => $"{FirstName} {LastName}";
    public string DisplayName => $"{LastName}, {FirstName}";
} 