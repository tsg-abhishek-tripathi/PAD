using System;

namespace PAD.Core.Entities;

public class WorkdaySyncStatus
{
    public int SyncId { get; set; }
    public string SyncType { get; set; } = string.Empty; // Full, Delta
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty; // Running, Completed, Failed
    public int? RecordsProcessed { get; set; }
    public int? RecordsUpdated { get; set; }
    public int? RecordsErrors { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedDate { get; set; }
} 