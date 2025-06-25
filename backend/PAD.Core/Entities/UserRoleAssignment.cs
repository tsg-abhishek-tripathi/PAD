using System;
using System.ComponentModel.DataAnnotations;

namespace PAD.Core.Entities;

public class UserRoleAssignment
{
    public int UserRoleAssignmentId { get; set; }
    public int UserId { get; set; }
    public int UserRoleId { get; set; }
    public int? LocationId { get; set; }
    
    [Required]
    public string Location { get; set; } = string.Empty;
    
    public DateTime AssignedDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual UserRole UserRole { get; set; } = null!;
    public virtual Office? LocationOffice { get; set; }
} 