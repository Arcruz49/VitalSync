using VitalSyncAPI.Domain.Entities;

namespace VitalSyncAPI.Domain.Interfaces;


public interface IBodyMetricsRepository
{
    Task<BodyMetrics?> GetLatestByUserId(Guid id);
    Task<BodyMetrics?> GetByUserId(Guid id);
    void UpdateMetrics(BodyMetrics metric);
    Task CreateMetrics(BodyMetrics metric);
    Task<List<BodyMetrics>> GetAllByUserId(Guid id);

}