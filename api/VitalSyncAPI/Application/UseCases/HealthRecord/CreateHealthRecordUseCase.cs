using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Domain.Enums;
using VitalSyncAPI.Domain.Services;

namespace VitalSyncAPI.Application.UseCases;

public class CreateHealthRecordUseCase (IHealthRecordsRepository recordRepository, IAlertRepository alertRepository,
    IMetricTypesRepository metricTypesRepository, IPersonalRangeRepository personalRangeRepository, 
    IUnitOfWork unitOfWork) : ICreateHealthRecordUseCase{

   
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
    

}
