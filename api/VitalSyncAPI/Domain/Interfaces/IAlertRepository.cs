using VitalSyncAPI.Domain.Entities;

namespace VitalSyncAPI.Domain.Interfaces;


public interface IAlertRepository
{
    Task AddAsync(Alert alert);
    Task <Alert> GetByIdAsync(Guid id);
    Task<List<Alert>> GetByUserAsync(Guid userId);
    Task DeleteByHealthRecordId(Guid id);
}