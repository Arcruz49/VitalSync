using MassTransit;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Enums;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Domain.Services;
using VitalSync.Contracts;

namespace VitalSyncAPI.Application.UseCases;

public class CreateHealthRecordUseCase(
    IHealthRecordsRepository recordRepository,
    IAlertRepository alertRepository,
    IMetricTypesRepository metricTypesRepository,
    IPersonalRangeRepository personalRangeRepository,
    IUserProfileRepository userProfileRepository,
    IBodyMetricsRepository bodyMetricsRepository,
    IUserConditionRepository userConditionRepository,
    IUserMedicationRepository userMedicationRepository,
    IHealthRecordsRepository healthRecordsRepository,
    IPublishEndpoint publishEndpoint,
    IUnitOfWork unitOfWork,
    ILogger<CreateHealthRecordUseCase> logger
) : ICreateHealthRecordUseCase
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

        await PublishInsightRequestAsync(userId, record, metricType);

        return new HealthRecordResponse(
            record.Id,
            record.MetricTypeId,
            metricType.Name,
            metricType.Unit,
            record.Value,
            record.MeasuredAt,
            record.Notes,
            record.CreatedAt,
            null
        );
    }

    private async Task PublishInsightRequestAsync(Guid userId, HealthRecord record, Domain.Entities.MetricType metricType)
    {
        try
        {
            var profile = await userProfileRepository.GetByUserId(userId);
            var metrics = await bodyMetricsRepository.GetLatestByUserId(userId);
            var conditions = await userConditionRepository.GetByUserId(userId);
            var medications = await userMedicationRepository.GetByUserId(userId);
            var recentRecords = await healthRecordsRepository.GetByUserAsync(
                userId, null,
                DateTime.UtcNow.AddDays(-30),
                null);

            if (profile is null || metrics is null) return;

            var recentMetrics = recentRecords
                .GroupBy(r => r.MetricType.Name)
                .Select(g => new MetricSummary(
                    g.Key,
                    g.First().MetricType.Unit,
                    g.OrderByDescending(r => r.MeasuredAt).First().Value,
                    g.Average(r => r.Value)
                )).ToList();

            await publishEndpoint.Publish(new InsightRequestedEvent(
                record.Id,
                userId,
                metricType.Name,
                metricType.Unit,
                record.Value,
                record.MeasuredAt,
                profile.Goal.ToString(),
                profile.ActivityLevel.ToString(),
                metrics.BMI,
                metrics.TDEE,
                metrics.CalorieGoal,
                conditions.Select(c => c.Condition.Name).ToList(),
                medications.Select(m => m.MedicationClass.ToString()).ToList(),
                recentMetrics
            ));
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Erro ao publicar InsightRequestedEvent para HealthRecord {Id}",
                record.Id);
        }
    }
}