using System.Collections.Generic;

namespace PAD.Core.Entities;

public class User
{
    public int UserId { get; set; }
    public required string UserPrincipalName { get; set; }
    public required string DisplayName { get; set; }
    public required string Email { get; set; }
    public int? EmployeeId { get; set; }
    
    // Navigation properties
    public required Employee Employee { get; set; }
    public required ICollection<UserRoleAssignment> UserRoleAssignments { get; set; }
} 