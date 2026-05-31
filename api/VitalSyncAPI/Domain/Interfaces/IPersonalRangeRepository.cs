using VitalSyncAPI.Domain.Entities;

namespace VitalSyncAPI.Domain.Interfaces;

public interface IPersonalRangeRepository
{
    Task<List<PersonalRange>> GetByUserId(Guid userId);
    Task<PersonalRange> GetByUserIdAndMetricId(Guid userId, int metricId);
    Task AddPersonalRanges(List<PersonalRange> personalRanges);
    Task RemovePersonalRange(Guid userId, int metricId);
    Task RemoveAllByUserId(Guid userId);
}
