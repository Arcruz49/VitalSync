using MassTransit;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Enums;
using VitalSyncAPI.Domain.Exceptions;
using VitalSyncAPI.Domain.Interfaces;
using VitalSync.Contracts;

namespace VitalSyncAPI.Application.UseCases;

public class AddWeeklyReportUseCase(
    IWeeklyReportRepository weeklyReportRepository,
    IHealthRecordsRepository healthRecordsRepository,
    INutritionRecordRepository nutritionRecordRepository,
    IUserProfileRepository userProfileRepository,
    IBodyMetricsRepository bodyMetricsRepository,
    IUserConditionRepository userConditionRepository,
    IUserMedicationRepository userMedicationRepository,
    IPublishEndpoint publishEndpoint,
    IUnitOfWork unitOfWork
) : IAddWeeklyReportUseCase
{
    public async Task<WeeklyReportResponse> ExecuteAsync(Guid userId, WeeklyReportRequest request)
    {
        var today = DateTime.UtcNow.Date;
        var weekStart = request.WeekStart?.Date
            ?? today.AddDays(-(((int)today.DayOfWeek + 6) % 7));
        var weekEnd = weekStart.AddDays(7).AddTicks(-1);

        var existing = await weeklyReportRepository.GetByUserIdAndWeek(userId, weekStart);
        if (existing is not null && existing.Status != ReportStatus.Failed)
            throw new ValidationException("Você já gerou o relatório desta semana.");

        var profile    = await userProfileRepository.GetByUserId(userId);
        var metrics    = await bodyMetricsRepository.GetLatestByUserId(userId);
        var conditions = await userConditionRepository.GetByUserId(userId);
        var medications= await userMedicationRepository.GetByUserId(userId);

        var weekRecords = await healthRecordsRepository.GetByUserAsync(userId, null, weekStart, weekEnd);
        var prevRecords = await healthRecordsRepository.GetByUserAsync(userId, null, weekStart.AddDays(-7), weekStart);
        var nutritionRecords = await nutritionRecordRepository.GetByUserId(userId, weekStart, weekEnd);

        var weekMetrics = BuildMetricData(weekRecords);
        var prevMetrics = BuildMetricData(prevRecords);

        var report = new WeeklyReport
        {
            Id        = Guid.NewGuid(),
            UserId    = userId,
            WeekStart = weekStart,
            WeekEnd   = weekEnd,
            Status    = ReportStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await weeklyReportRepository.AddWeeklyReport(report);
        await unitOfWork.SaveChangesAsync();

        await publishEndpoint.Publish(new WeeklyReportRequestedEvent(
            report.Id,
            userId,
            weekStart,
            weekEnd,
            profile?.Goal.ToString() ?? string.Empty,
            profile?.ActivityLevel.ToString() ?? string.Empty,
            metrics?.BMI ?? 0,
            metrics?.CalorieGoal ?? 0,
            conditions.Select(c => c.Condition.Name).ToList(),
            medications.Select(m => m.MedicationClass.ToString()).ToList(),
            weekMetrics,
            nutritionRecords.Sum(n => n.CaloriesKcal),
            nutritionRecords.Sum(n => n.ProteinG),
            nutritionRecords.Sum(n => n.CarbsG),
            nutritionRecords.Sum(n => n.FatG),
            nutritionRecords.Count,
            prevMetrics
        ));

        return new WeeklyReportResponse(
            report.Id,
            report.WeekStart,
            report.WeekEnd,
            "Gerando relatório...",
            [],
            [],
            null,
            string.Empty,
            report.Status.ToString(),
            report.CreatedAt,
            null
        );
    }

    private static List<WeeklyMetricData> BuildMetricData(List<Domain.Entities.HealthRecord> records)
    {
        return records
            .GroupBy(r => r.MetricType.Name)
            .Select(g =>
            {
                var ordered = g.OrderBy(r => r.MeasuredAt).ToList();
                var avg = g.Average(r => r.Value);
                var firstHalf = ordered.Take(ordered.Count / 2).Average(r => r.Value);
                var secondHalf = ordered.Skip(ordered.Count / 2).Average(r => r.Value);
                var trend = secondHalf > firstHalf * 1.05m ? "worsening"
                          : secondHalf < firstHalf * 0.95m ? "improving"
                          : "stable";

                return new WeeklyMetricData(
                    g.Key,
                    g.First().MetricType.Unit,
                    Math.Round(avg, 1),
                    g.Min(r => r.Value),
                    g.Max(r => r.Value),
                    ordered.First().Value,
                    ordered.Last().Value,
                    g.Count(),
                    trend
                );
            }).ToList();
    }
}