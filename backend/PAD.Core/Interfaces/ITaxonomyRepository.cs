using PAD.Core.Entities;

namespace PAD.Core.Interfaces;

public interface ITaxonomyRepository : IRepository<Taxonomy>
{
    Task<IEnumerable<Taxonomy>> GetTaxonomyByTypeAsync(string taxonomyType);
    Task<IEnumerable<Taxonomy>> GetActiveTaxonomyAsync();
    Task<IEnumerable<Taxonomy>> GetTaxonomyHierarchyAsync(int parentId);
} 