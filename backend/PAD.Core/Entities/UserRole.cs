using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PAD.Core.Entities;

public class UserRole
{
    public int UserRoleId { get; set; }
    
    [Required]
    [StringLength(50)]
    public required string RoleName { get; set; }
    
    [Required]
    [StringLength(200)]
    public required string Description { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public string CreatedBy { get; set; } = string.Empty;
    public string ModifiedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    
    // Navigation properties
    public required ICollection<UserRoleAssignment> UserRoleAssignments { get; set; } = new List<UserRoleAssignment>();
} 