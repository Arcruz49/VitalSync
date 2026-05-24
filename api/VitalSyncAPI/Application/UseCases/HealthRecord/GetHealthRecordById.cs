using System.Data.Common;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;

namespace VitalSyncAPI.Application.UseCases;

public class GetHealthRecordById (IHealthRecordsRepository recordRepository) : IGetHealthRecordById{

   
    public async Task<HealthRecordResponse> ExecuteAsync(Guid id)
    {
        var record = await recordRepository.GetById(id);
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
