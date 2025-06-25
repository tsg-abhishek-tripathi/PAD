using PAD.Core.Entities;

namespace PAD.Core.Interfaces;

public interface IOfficeRepository : IRepository<Office>
{
    Task<IEnumerable<Office>> GetOfficesByRegionAsync(string region);
    Task<IEnumerable<Office>> GetActiveOfficesAsync();
    Task<Office?> GetOfficeWithEmployeesAsync(int officeId);
} 