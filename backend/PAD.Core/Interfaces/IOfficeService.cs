using PAD.Core.Entities;

namespace PAD.Core.Interfaces;

public interface IOfficeService
{
    Task<Office?> GetOfficeAsync(int officeId);
    Task<IEnumerable<Office>> GetOfficesByRegionAsync(string region);
    Task<Office> CreateOfficeAsync(Office office);
    Task UpdateOfficeAsync(Office office);
    Task DeleteOfficeAsync(int officeId);
} 