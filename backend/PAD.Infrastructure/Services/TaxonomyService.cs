using PAD.Core.Entities;
using PAD.Core.Interfaces;

namespace PAD.Infrastructure.Services;

public class TaxonomyService : ITaxonomyService
{
    private readonly ITaxonomyRepository _taxonomyRepository;
    private readonly IAuditService _auditService;

    public TaxonomyService(ITaxonomyRepository taxonomyRepository, IAuditService auditService)
    {
        _taxonomyRepository = taxonomyRepository;
        _auditService = auditService;
    }

    public async Task<Taxonomy?> GetTaxonomyAsync(int taxonomyId)
    {
        return await _taxonomyRepository.GetByIdAsync(taxonomyId);
    }

    public async Task<IEnumerable<Taxonomy>> GetTaxonomyByTypeAsync(string taxonomyType)
    {
        return await _taxonomyRepository.GetTaxonomyByTypeAsync(taxonomyType);
    }

    public async Task<Taxonomy> CreateTaxonomyAsync(Taxonomy taxonomy)
    {
        var result = await _taxonomyRepository.AddAsync(taxonomy);
        await _auditService.LogAuditAsync("Taxonomy", "Create", taxonomy.TaxonomyId.ToString());
        return result;
    }

    public async Task UpdateTaxonomyAsync(Taxonomy taxonomy)
    {
        await _taxonomyRepository.UpdateAsync(taxonomy);
        await _auditService.LogAuditAsync("Taxonomy", "Update", taxonomy.TaxonomyId.ToString());
    }

    public async Task DeleteTaxonomyAsync(int taxonomyId)
    {
        var taxonomy = await _taxonomyRepository.GetByIdAsync(taxonomyId);
        if (taxonomy != null)
        {
            await _taxonomyRepository.DeleteAsync(taxonomy);
            await _auditService.LogAuditAsync("Taxonomy", "Delete", taxonomyId.ToString());
        }
    }
} 