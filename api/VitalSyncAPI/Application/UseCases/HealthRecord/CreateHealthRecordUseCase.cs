using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Domain.Enums;

namespace VitalSyncAPI.Application.UseCases;

public class CreateHealthRecordUseCase (IHealthRecordsRepository recordRepository, IAlertRepository alertRepository,
    IMetricTypesRepository metricTypesRepository, IUnitOfWork unitOfWork) : ICreateHealthRecordUseCase{

   
    public async Task<HealthRecordResponse> ExecuteAsync(Guid userId, HealthRecordRequest request)
    {
        var metricType = await metricTypesRepository.GetById(request.MetricTypeId);

        var record = new HealthRecord
        {
            Id           = Guid.NewGuid(),
            UserId       = userId,
            MetricTypeId = request.MetricTypeId,
            Value        = request.Value,
            MeasuredAt   = request.MeasuredAt,
            Notes        = request.Notes,
            CreatedAt    = DateTime.UtcNow
        };

        await recordRepository.AddAsync(record);

        var alert = GenerateAlertIfNeeded(record, metricType);
        if (alert is not null) await alertRepository.AddAsync(alert);

        await unitOfWork.SaveChangesAsync();

        return new HealthRecordResponse(
            record.Id,
            record.MetricTypeId,
            metricType.Name,
            metricType.Unit,
            record.Value,
            record.MeasuredAt,
            record.Notes,
            record.CreatedAt
        );
    }

    private static Alert? GenerateAlertIfNeeded(HealthRecord record, MetricType metricType)
    {
        if (metricType.MinNormal is null && metricType.MaxNormal is null) return null;

        var aboveMax = metricType.MaxNormal.HasValue && record.Value > (decimal)metricType.MaxNormal.Value;
        var belowMin = metricType.MinNormal.HasValue && record.Value < (decimal)metricType.MinNormal.Value;

        if (!aboveMax && !belowMin) return null;

        var severity = CalculateSeverity(record.Value, metricType, aboveMax);
        var message = BuildAlertMessage(record.Value, metricType, aboveMax);

        return new Alert
        {
            Id = Guid.NewGuid(),
            UserId = record.UserId,
            HealthRecordId = record.Id,
            MetricTypeId = record.MetricTypeId,
            Severity = severity,
            Message = message,
            TriggeredAt = DateTime.UtcNow
        };
    }

    private static string BuildAlertMessage(decimal value, MetricType metricType, bool isAbove)
    {
        var direction = isAbove ? "acima" : "abaixo";
        var limit     = isAbove ? metricType.MaxNormal : metricType.MinNormal;

        return $"{metricType.Name} registrada {direction} do normal: " +
               $"{value} {metricType.Unit} (limite: {limit} {metricType.Unit}).";
    }

    private static AlertSeverity CalculateSeverity(decimal value, MetricType metricType, bool isAbove)
    {
        var limit = isAbove ? (decimal)metricType.MaxNormal!.Value : (decimal)metricType.MinNormal!.Value;

        var deviation = Math.Abs((value - limit) / limit * 100);

        return deviation >= 20 ? AlertSeverity.Critical : AlertSeverity.Warning;
    }

}
