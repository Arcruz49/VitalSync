using System.Data.Common;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Exceptions;
using VitalSyncAPI.Domain.Interfaces;

namespace VitalSyncAPI.Application.UseCases;

public class GetHealthRecordById (IHealthRecordsRepository recordRepository) : IGetHealthRecordById{

   
    public async Task<HealthRecordResponse> ExecuteAsync(Guid userId, Guid id)
    {
        var record = await recordRepository.GetById(id);

         if (record.UserId != userId) throw new ForbiddenException("Você não tem permissão para acessar este registro.");


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
