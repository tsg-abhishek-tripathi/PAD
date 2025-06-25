using System;
using System.ComponentModel.DataAnnotations;

namespace PAD.Core.Entities;

public class ActivityLog
{
    public int ActivityLogId { get; set; }
    public int ActivityId { get; set; }
    
    [Required]
    public string ActivityType { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public string EntityType { get; set; } = string.Empty;
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    public DateTime Timestamp { get; set; }
    
    [Required]
    public string Details { get; set; } = string.Empty;
    
    [Required]
    public string Status { get; set; } = string.Empty;
    
    public string? ErrorMessage { get; set; }
    
    [Required]
    public string IPAddress { get; set; } = string.Empty;
    
    [Required]
    public string UserAgent { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
} 