using VitalSyncAPI.Domain.Entities;

namespace VitalSyncAPI.Domain.Interfaces;


public interface IHealthRecordsRepository
{
    Task<HealthRecord> AddAsync(HealthRecord record);
    void Edit(HealthRecord record);
    Task Delete(Guid id);
    Task <HealthRecord> GetById(Guid id);
    Task<List<HealthRecord>> GetByUserAsync(Guid userId, int? metricTypeId, DateTime? from, DateTime? to);
}