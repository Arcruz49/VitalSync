using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Domain.Enums;
using VitalSyncAPI.Domain.Services;
using VitalSyncAPI.Application.Models;
using System.Text.Json;

namespace VitalSyncAPI.Application.UseCases;

public class CreateHealthRecordUseCase(
    IHealthRecordsRepository recordRepository,
    IAlertRepository alertRepository,
    IMetricTypesRepository metricTypesRepository,
    IPersonalRangeRepository personalRangeRepository,
    IAIInsightRepository aiInsightRepository,
    IUserRepository userRepository,
    IBodyMetricsRepository bodyMetricsRepository,
    IAIAnalysisService aiAnalysisService,
    IUnitOfWork unitOfWork) : ICreateHealthRecordUseCase
{
    public async Task<HealthRecordResponse> ExecuteAsync(Guid userId, HealthRecordRequest request)
    {
        var metricType = await metricTypesRepository.GetById(request.MetricTypeId);
        var personalRange = await personalRangeRepository.GetByUserIdAndMetricId(userId, request.MetricTypeId);

        var record = new HealthRecord
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            MetricTypeId = request.MetricTypeId,
            Value = request.Value,
            MeasuredAt = request.MeasuredAt,
            Notes = request.Notes,
            Source = RecordSource.Manual,
            CreatedAt = DateTime.UtcNow
        };

        await recordRepository.AddAsync(record);

        var alert = personalRange is not null
            ? AlertGenerator.Generate(record, metricType, personalRange.MinNormal, personalRange.MaxNormal)
            : AlertGenerator.Generate(record, metricType, (decimal?)metricType.MinNormal, (decimal?)metricType.MaxNormal);

        if (alert is not null)
            await alertRepository.AddAsync(alert);

        await unitOfWork.SaveChangesAsync();

        // busca contexto e gera insight
        var user = await userRepository.GetUserById(userId);
        var metrics = await bodyMetricsRepository.GetLatestByUserId(userId);
        var recentRecords = await recordRepository.GetByUserAsync(userId, null, DateTime.UtcNow.AddDays(-30), null);

        AIAnalysisResult? analysisResult = null;

        if (user.Profile is not null && metrics is not null)
        {
            analysisResult = await aiAnalysisService.AnalyzeAsync(user.Profile, metrics, recentRecords);

            var insight = new AIInsight
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                HealthRecordId = record.Id,
                Insights = JsonSerializer.Serialize(analysisResult.Insights),
                Tips = JsonSerializer.Serialize(analysisResult.Tips),
                OverallAssessment = analysisResult.OverallAssessment,
                Disclaimer = analysisResult.Disclaimer,
                CreatedAt = DateTime.UtcNow
            };

            await aiInsightRepository.AddAsync(insight);
            await unitOfWork.SaveChangesAsync();
        }

        return new HealthRecordResponse(
            record.Id,
            record.MetricTypeId,
            metricType.Name,
            metricType.Unit,
            record.Value,
            record.MeasuredAt,
            record.Notes,
            record.CreatedAt,
            analysisResult
        );
    }
}
