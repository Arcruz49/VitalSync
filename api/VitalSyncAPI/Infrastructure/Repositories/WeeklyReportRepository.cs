using Microsoft.EntityFrameworkCore;
using VitalSyncAPI.Infrastructure.Data;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Domain.Exceptions;

namespace VitalSyncAPI.Infrastructure.Repositories;

public class WeeklyReportRepository(Context db) : BaseRepository<WeeklyReport>(db), IWeeklyReportRepository
{
    public async Task<WeeklyReport?> GetByUserIdAndWeek(Guid userId, DateTime weekStart)
    {
        return await Query()
            .Where(x => x.UserId == userId && x.WeekStart.Date == weekStart.Date)
            .FirstOrDefaultAsync();
    }

    public async Task<WeeklyReport?> GetById(Guid id)
    {
        return await Query()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<WeeklyReport>> GetByUserId(Guid userId)
    {
        return await Query()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.WeekStart)
            .ToListAsync();
    }

    public async Task AddWeeklyReport(WeeklyReport weeklyReport)
    {
        await AddAsync(weeklyReport);
    }

    public void UpdateWeeklyReport(WeeklyReport weeklyReport)
    {
        Update(weeklyReport);
    }
}