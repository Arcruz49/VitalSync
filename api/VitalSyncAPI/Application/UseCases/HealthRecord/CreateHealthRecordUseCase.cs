using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.UseCases;

public class CreateHealthRecordUseCase (IHealthRecordsRepository recordRepository, IUnitOfWork unitOfWork) : ICreateHealthRecordUseCase{

   
    public async Task<HealthRecordResponse> ExecuteAsync(Guid userId, HealthRecordRequest request)
    {
        var record = new HealthRecord()
        {
            UserId = userId,
            MetricTypeId = request.MetricTypeId,
            Value = request.Value,
            MeasuredAt = request.MeasuredAt,
            CreatedAt = DateTime.UtcNow,
            Notes = request.Notes,
        };

        await recordRepository.AddAsync(record);
        await unitOfWork.SaveChangesAsync();
        return new HealthRecordResponse(
            record.Id,
            record.MetricTypeId,
            record.MetricType.Name,  
            record.MetricType.Unit, 
            record.Value,
            record.MeasuredAt,
            record.Notes,
            record.CreatedAt
        );
    }

}
