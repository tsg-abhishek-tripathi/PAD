using PAD.Core.Entities;
using PAD.Core.Interfaces;

namespace PAD.Infrastructure.Services;

public class OfficeService : IOfficeService
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IAuditService _auditService;

    public OfficeService(IOfficeRepository officeRepository, IAuditService auditService)
    {
        _officeRepository = officeRepository;
        _auditService = auditService;
    }

    public async Task<Office?> GetOfficeAsync(int officeId)
    {
        return await _officeRepository.GetByIdAsync(officeId);
    }

    public async Task<IEnumerable<Office>> GetOfficesByRegionAsync(string region)
    {
        return await _officeRepository.GetOfficesByRegionAsync(region);
    }

    public async Task<Office> CreateOfficeAsync(Office office)
    {
        var result = await _officeRepository.AddAsync(office);
        await _auditService.LogAuditAsync("Office", "Create", office.OfficeId.ToString());
        return result;
    }

    public async Task UpdateOfficeAsync(Office office)
    {
        await _officeRepository.UpdateAsync(office);
        await _auditService.LogAuditAsync("Office", "Update", office.OfficeId.ToString());
    }

    public async Task DeleteOfficeAsync(int officeId)
    {
        var office = await _officeRepository.GetByIdAsync(officeId);
        if (office != null)
        {
            await _officeRepository.DeleteAsync(office);
            await _auditService.LogAuditAsync("Office", "Delete", officeId.ToString());
        }
    }
} 