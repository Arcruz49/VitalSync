using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Responses;
using System.Text.Json;

namespace VitalSyncAPI.Application.UseCases;

public class GetWeeklyReportsUseCase(
    IWeeklyReportRepository weeklyReportRepository
    ) : IGetWeeklyReportsUseCase
{
    public async Task<List<WeeklyReportResponse>> ExecuteAsync(Guid userId)
    {
        var reports = await weeklyReportRepository.GetByUserId(userId);

        return reports.Select(report => new WeeklyReportResponse(
            report.Id,
            report.WeekStart,
            report.WeekEnd,
            report.Summary,
            JsonSerializer.Deserialize<List<string>>(report.Patterns) ?? [],
            JsonSerializer.Deserialize<List<string>>(report.Recommendations) ?? [],
            report.NutritionSummary,
            report.Disclaimer,
            report.Status.ToString(),
            report.CreatedAt,
            null // MetricsAnalysis só no detalhe
        )).ToList();
    }
}