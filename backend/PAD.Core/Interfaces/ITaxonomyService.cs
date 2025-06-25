using PAD.Core.Entities;

namespace PAD.Core.Interfaces;

public interface ITaxonomyService
{
    Task<Taxonomy?> GetTaxonomyAsync(int taxonomyId);
    Task<IEnumerable<Taxonomy>> GetTaxonomyByTypeAsync(string taxonomyType);
    Task<Taxonomy> CreateTaxonomyAsync(Taxonomy taxonomy);
    Task UpdateTaxonomyAsync(Taxonomy taxonomy);
    Task DeleteTaxonomyAsync(int taxonomyId);
} 