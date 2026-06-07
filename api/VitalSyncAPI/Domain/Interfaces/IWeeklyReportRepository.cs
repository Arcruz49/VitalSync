using VitalSyncAPI.Domain.Entities;

namespace VitalSyncAPI.Domain.Interfaces;

public interface IWeeklyReportRepository
{
    Task<WeeklyReport?> GetByUserIdAndWeek(Guid userId, DateTime weekStart);
    Task<WeeklyReport?> GetById(Guid id);
    Task<List<WeeklyReport>> GetByUserId(Guid userId);
    Task AddWeeklyReport(WeeklyReport weeklyReport);
    void UpdateWeeklyReport(WeeklyReport weeklyReport);
}