using VitalSyncAPI.Domain.Entities;

namespace VitalSyncAPI.Domain.Interfaces;


public interface IAIInsightRepository
{
    Task AddAsync(AIInsight aiInsight);
    Task DeleteByHealthRecordId(Guid healthRecordId);
}