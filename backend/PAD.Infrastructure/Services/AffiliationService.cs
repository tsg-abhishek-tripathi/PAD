using PAD.Core.Entities;
using PAD.Core.Interfaces;
using PAD.Core.Models;
using ValidationError = PAD.Core.Models.ValidationError;
using BulkAffiliationRequest = PAD.Core.Models.BulkAffiliationRequest;

namespace PAD.Infrastructure.Services;

public class AffiliationService : IAffiliationService
{
    private readonly IAffiliationRepository _affiliationRepository;
    private readonly IAuditService _auditService;

    public AffiliationService(IAffiliationRepository affiliationRepository, IAuditService auditService)
    {
        _affiliationRepository = affiliationRepository;
        _auditService = auditService;
    }

    public async Task<EmployeeAffiliation?> GetAffiliationAsync(int affiliationId)
    {
        return await _affiliationRepository.GetByIdAsync(affiliationId);
    }

    public async Task<IEnumerable<EmployeeAffiliation>> GetEmployeeAffiliationsAsync(int employeeId)
    {
        return await _affiliationRepository.GetAffiliationsByEmployeeAsync(employeeId);
    }

    public async Task<IEnumerable<EmployeeAffiliation>> GetAffiliationsByPracticeAsync(int practiceId, int userId)
    {
        return await _affiliationRepository.GetAffiliationsByPracticeAsync(practiceId);
    }

    public async Task<EmployeeAffiliation> CreateAffiliationAsync(EmployeeAffiliation affiliation)
    {
        var result = await _affiliationRepository.AddAsync(affiliation);
        await _auditService.LogAuditAsync("EmployeeAffiliation", "Create", affiliation.AffiliationId.ToString());
        return result;
    }

    public async Task UpdateAffiliationAsync(EmployeeAffiliation affiliation)
    {
        await _affiliationRepository.UpdateAsync(affiliation);
        await _auditService.LogAuditAsync("EmployeeAffiliation", "Update", affiliation.AffiliationId.ToString());
    }

    public async Task DeleteAffiliationAsync(int affiliationId)
    {
        var affiliation = await _affiliationRepository.GetByIdAsync(affiliationId);
        if (affiliation != null)
        {
            await _affiliationRepository.DeleteAsync(affiliation);
            await _auditService.LogAuditAsync("EmployeeAffiliation", "Delete", affiliationId.ToString());
        }
    }

    public async Task<List<ValidationError>> ValidateAffiliationAsync(int employeeId, string roleTypeCode, string practiceCode)
    {
        var errors = new List<ValidationError>();
        
        // Validate employee exists
        var existingAffiliations = await GetEmployeeAffiliationsAsync(employeeId);
        
        // Check for duplicate affiliations
        if (existingAffiliations.Any(a => 
            a.RoleTypeCode == roleTypeCode && 
            a.PracticeCode == practiceCode &&
            a.IsActive))
        {
            errors.Add(new ValidationError 
            { 
                Field = "Affiliation",
                Code = "DuplicateAffiliation",
                Message = "An active affiliation with this role and practice already exists."
            });
        }

        return errors;
    }

    public async Task<bool> CanUserEditAffiliationAsync(int affiliationId, int userId)
    {
        var affiliation = await GetAffiliationAsync(affiliationId);
        if (affiliation == null) return false;
        
        // Add authorization logic here - for now returning true
        return true;
    }

    public async Task BulkUpdateAffiliationsAsync(BulkAffiliationRequest request, int userId)
    {
        foreach (var employeeId in request.EmployeeIds)
        {
            try
            {
                switch (request.Action.ToLower())
                {
                    case "add":
                        var newAffiliation = new EmployeeAffiliation
                        {
                            EmployeeId = employeeId,
                            RoleTypeCode = request.RoleTypeCode,
                            PracticeCode = request.PracticeCode ?? string.Empty,
                            LocationScope = request.LocationScope,
                            EffectiveDate = request.EffectiveDate ?? DateTime.Today,
                            ExpirationDate = request.ExpirationDate,
                            IsActive = true,
                            Source = "Bulk",
                            CreatedBy = userId.ToString(),
                            ModifiedBy = userId.ToString(),
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow
                        };
                        await CreateAffiliationAsync(newAffiliation);
                        break;

                    case "update":
                        var existingAffiliations = await GetEmployeeAffiliationsAsync(employeeId);
                        var targetAffiliation = existingAffiliations.FirstOrDefault(a => 
                            a.RoleTypeCode == request.RoleTypeCode && 
                            a.PracticeCode == request.PracticeCode);
                        
                        if (targetAffiliation != null)
                        {
                            targetAffiliation.LocationScope = request.LocationScope;
                            targetAffiliation.EffectiveDate = request.EffectiveDate ?? targetAffiliation.EffectiveDate;
                            targetAffiliation.ExpirationDate = request.ExpirationDate;
                            targetAffiliation.ModifiedBy = userId.ToString();
                            targetAffiliation.ModifiedDate = DateTime.UtcNow;
                            await UpdateAffiliationAsync(targetAffiliation);
                        }
                        break;

                    case "remove":
                        var affiliationsToRemove = await GetEmployeeAffiliationsAsync(employeeId);
                        var affiliationToRemove = affiliationsToRemove.FirstOrDefault(a => 
                            a.RoleTypeCode == request.RoleTypeCode && 
                            a.PracticeCode == request.PracticeCode);
                        
                        if (affiliationToRemove != null)
                        {
                            await DeleteAffiliationAsync(affiliationToRemove.AffiliationId);
                        }
                        break;
                }

                await _auditService.LogAuditAsync(
                    "EmployeeAffiliation",
                    $"Bulk{request.Action}",
                    $"Employee: {employeeId}",
                    request.ChangeReason);
            }
            catch (Exception ex)
            {
                // Log error and continue with next employee
                await _auditService.LogAuditAsync(
                    "EmployeeAffiliation",
                    "BulkUpdateError",
                    $"Employee: {employeeId}",
                    ex.Message);
            }
        }
    }
} 