using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Domain.Enums;
using System.Text.Json;
using VitalSyncAPI.Domain.Exceptions;

namespace VitalSyncAPI.Application.UseCases;

public class GetWeeklyReportUseCase(
    IWeeklyReportRepository weeklyReportRepository
    ) : IGetWeeklyReportUseCase
{
    public async Task<WeeklyReportResponse> ExecuteAsync(Guid userId, DateTime dateTime)
    {
        var report = await weeklyReportRepository.GetByUserIdAndWeek(userId, dateTime)
            ?? throw new NotFoundException("Relatório não encontrado.");

        var metricsAnalysis = string.IsNullOrEmpty(report.MetricsAnalysis)
            ? null
            : JsonSerializer.Deserialize<List<WeeklyMetricAnalysisResponse>>(report.MetricsAnalysis,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return new WeeklyReportResponse(
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
            metricsAnalysis
        );
    }
}