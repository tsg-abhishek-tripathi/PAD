using Microsoft.EntityFrameworkCore;
using PAD.Core.Entities;
using PAD.Core.Interfaces;
using PAD.Infrastructure.Data;

namespace PAD.Infrastructure.Repositories;

public class TaxonomyRepository : Repository<Taxonomy>, ITaxonomyRepository
{
    private readonly new PadDbContext _context;

    public TaxonomyRepository(PadDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Taxonomy>> GetByTypeAsync(string facetType)
    {
        return await _context.Taxonomies
            .Where(t => t.FacetType == facetType && t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Taxonomy>> GetTaxonomyByTypeAsync(string taxonomyType)
    {
        return await _context.Taxonomies
            .Where(t => t.FacetType == taxonomyType && t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Taxonomy>> GetActiveTaxonomyAsync()
    {
        return await _context.Taxonomies
            .Where(t => t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Taxonomy>> GetTaxonomyHierarchyAsync(int parentId)
    {
        return await _context.Taxonomies
            .Where(t => t.ParentId == parentId && t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }
} 