using VitalSyncAPI.Domain.Entities;

namespace VitalSyncAPI.Domain.Interfaces;


public interface IMetricTypesRepository
{
    Task<List<MetricType>> GetAll();
    Task<MetricType> GetById(int id);
}