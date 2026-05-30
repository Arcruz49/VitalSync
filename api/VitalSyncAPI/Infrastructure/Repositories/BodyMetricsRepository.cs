using Microsoft.EntityFrameworkCore;
using VitalSyncAPI.Infrastructure.Data;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Domain.Exceptions;

namespace VitalSyncAPI.Infrastructure.Repositories;

public class UserMetricRepository(Context db) : BaseRepository<BodyMetrics>(db), IBodyMetricsRepository
{
    public async Task<BodyMetrics?> GetLatestByUserId(Guid id)
    {
        return await Query()
            .Where(x => x.UserId == id)
            .OrderByDescending(x => x.RecordedAt)
            .FirstOrDefaultAsync();
    }
    public async Task<List<BodyMetrics>> GetAllByUserId(Guid id)
    {
        return await Query()
            .Where(x => x.UserId == id)
            .OrderByDescending(x => x.RecordedAt)
            .ToListAsync();
    }

    public async Task<BodyMetrics?> GetByUserId(Guid id)
    {
        return await Query().FirstOrDefaultAsync(x => x.UserId == id);
    }

    public async Task<BodyMetrics> GetMetricsById(Guid id)
    {
        return await Query()
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException("Métricas não encontradas.");
    }

    public async Task CreateMetrics(BodyMetrics metric)
    {
        await AddAsync(metric);
    }

    public void UpdateMetrics(BodyMetrics metric)
    {
        Update(metric);
    }

}