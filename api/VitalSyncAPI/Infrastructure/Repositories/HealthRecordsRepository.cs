using Microsoft.EntityFrameworkCore;
using VitalSyncAPI.Infrastructure.Data;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace VitalSyncAPI.Infrastructure.Repositories;

public class HealthRecordsRepository(Context db) : IHealthRecordsRepository{

    public async Task<HealthRecord> AddAsync(HealthRecord record)
    {
        await db.AddAsync(record);
        return record;
    }
    public void Edit(HealthRecord record)
    {
        db.Update(record);
    }
    public async Task Delete(Guid id)
    {
        var record = await GetById(id);
        db.Remove(record);
    }
    public async Task <HealthRecord> GetById(Guid id)
    {
        return await db.HealthRecords.Where(a => a.Id == id).Include(x => x.MetricType).Include(x => x.AIInsight).FirstOrDefaultAsync() ?? throw new NotFoundException("Registro não encontrado");
    }
    public async Task<List<HealthRecord>> GetByUserAsync(Guid userId, int? metricTypeId, DateTime? from, DateTime? to)
    {
        var records = db.HealthRecords
            .AsNoTracking()
            .Include(x => x.MetricType)
            .Include(x => x.AIInsight)
            .Where(x => x.UserId == userId);

        if (metricTypeId.HasValue) records = records.Where(x => x.MetricTypeId == metricTypeId.Value);
        if (from.HasValue) records = records.Where(x => x.MeasuredAt >= from.Value);
        if (to.HasValue) records = records.Where(x => x.MeasuredAt <= to.Value);

        return await records
            .OrderByDescending(x => x.MeasuredAt)
            .ToListAsync();
    }
}
