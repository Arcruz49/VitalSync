using Microsoft.EntityFrameworkCore;
using VitalSyncAPI.Infrastructure.Data;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Domain.Exceptions;

namespace VitalSyncAPI.Infrastructure.Repositories;

public class AlertRepository(Context db) : IAlertRepository
{
    public async Task AddAsync(Alert alert)
    {
        await db.AddAsync(alert);
    }

    public async Task<Alert> GetByIdAsync(Guid id)
    {
        return await db.Alerts
            .Include(x => x.MetricType)
            .Include(x => x.HealthRecord)
            .Where(a => a.Id == id)
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException("Alerta não encontrado.");
    }

    public async Task<List<Alert>> GetByUserAsync(Guid userId)
    {
        return await db.Alerts
            .AsNoTracking()
            .Include(x => x.MetricType)
            .Include(x => x.HealthRecord)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.TriggeredAt)
            .ToListAsync();
    }

    public async Task DeleteByHealthRecordId(Guid id)
    {
        var alert = await db.Alerts.Where(a => a.HealthRecordId == id).FirstAsync();
        db.Alerts.Remove(alert);
    }
    
}