using Microsoft.AspNetCore.Http;
using PAD.Core.Entities;
using PAD.Core.Interfaces;
using System.Security.Claims;

namespace PAD.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IRepository<AuditLog> _auditLogRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(IRepository<AuditLog> auditLogRepository, IHttpContextAccessor httpContextAccessor)
    {
        _auditLogRepository = auditLogRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAuditAsync(string entityType, string action, string entityId, string? details = null)
    {
        var auditLog = new AuditLog
        {
            EntityType = entityType,
            Action = action,
            EntityId = entityId,
            Details = details,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        await _auditLogRepository.AddAsync(auditLog);
    }

    public async Task LogChangeAsync(string entityType, string entityId, string field, string oldValue, string newValue, string userId)
    {
        var auditLog = new AuditLog
        {
            EntityType = entityType,
            Action = "Change",
            EntityId = entityId,
            Details = $"Field: {field}, Old: {oldValue}, New: {newValue}",
            CreatedBy = userId,
            ModifiedBy = userId,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        await _auditLogRepository.AddAsync(auditLog);
    }

    public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(string entityName, string entityId)
    {
        return await _auditLogRepository.GetAllAsync(a => 
            a.EntityType == entityName && 
            a.EntityId == entityId);
    }

    public async Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(string userId)
    {
        return await _auditLogRepository.GetAllAsync(a => 
            a.CreatedBy == userId);
    }
} 