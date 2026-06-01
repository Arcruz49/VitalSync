using Microsoft.EntityFrameworkCore;
using VitalSyncAPI.Infrastructure.Data;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Domain.Exceptions;

namespace VitalSyncAPI.Infrastructure.Repositories;

public class AIInsightRepository(Context db) : IAIInsightRepository
{
    public async Task AddAsync(AIInsight aiInsight)
    {
        await db.AIInsights.AddAsync(aiInsight);
    }

    public async Task DeleteByHealthRecordId(Guid healthRecordId)
    {
        var aiInsight = await db.AIInsights.Where(a => a.HealthRecordId == healthRecordId).FirstAsync();
        db.AIInsights.Remove(aiInsight);
    }
    
}