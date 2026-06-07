using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Domain.Enums;
using System.Text.Json;
using VitalSyncAPI.Domain.Exceptions;

namespace VitalSyncAPI.Application.UseCases;

public class GetWeeklyReportsByIdUseCase(
    IWeeklyReportRepository weeklyReportRepository
    ) : IGetWeeklyReportsByIdUseCase
{
    public async Task<WeeklyReportResponse> ExecuteAsync(Guid id, Guid userId)
    {
        var report = await weeklyReportRepository.GetById(id)
            ?? throw new NotFoundException("Relatório não encontrado.");

        if (report.UserId != userId)
            throw new ForbiddenException("Você não possui permissão para acessar esse relatório.");

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