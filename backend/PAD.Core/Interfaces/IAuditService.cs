using PAD.Core.Entities;

namespace PAD.Core.Interfaces;

public interface IAuditService
{
    Task LogAuditAsync(string entityType, string action, string entityId, string? details = null);
    Task LogChangeAsync(string entityType, string entityId, string field, string oldValue, string newValue, string userId);
    Task<IEnumerable<AuditLog>> GetAuditLogsAsync(string entityName, string entityId);
    Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(string userId);
}

public class AuditLogEntry
{
    public long AuditId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public int RecordId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string ChangeType { get; set; } = string.Empty;
    public string? ChangeReason { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime ChangedDate { get; set; }
    public string? UserAgent { get; set; }
    public string? IPAddress { get; set; }
}

public class ActivityLogEntry
{
    public long ActivityId { get; set; }
    public int? UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public string? Description { get; set; }
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedDate { get; set; }
} 