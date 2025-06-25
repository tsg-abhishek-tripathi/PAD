using System;
using System.ComponentModel.DataAnnotations;

namespace PAD.Core.Entities;

public class IntegrationLog
{
    public int IntegrationLogId { get; set; }
    public int IntegrationId { get; set; }
    
    [Required]
    public string SystemName { get; set; } = string.Empty;
    
    [Required]
    public string IntegrationType { get; set; } = string.Empty;
    
    [Required]
    public string RequestType { get; set; } = string.Empty;
    
    [Required]
    public string Endpoint { get; set; } = string.Empty;
    
    [Required]
    public string Operation { get; set; } = string.Empty;
    
    [Required]
    public string EntityType { get; set; } = string.Empty;
    
    [Required]
    public string EntityId { get; set; } = string.Empty;
    
    public DateTime Timestamp { get; set; }
    
    [Required]
    public string Status { get; set; } = string.Empty;
    
    [Required]
    public string RequestPayload { get; set; } = string.Empty;
    
    [Required]
    public string ResponsePayload { get; set; } = string.Empty;
    
    public string? ErrorMessage { get; set; }
} 