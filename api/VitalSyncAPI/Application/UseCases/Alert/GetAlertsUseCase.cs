using System.Data.Common;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Exceptions;
using VitalSyncAPI.Domain.Interfaces;

namespace VitalSyncAPI.Application.UseCases;

public class GetAlertsUseCase (IAlertRepository alertRepository) : IGetAlertsUseCase{

   
    public async Task<List<AlertResponse>> ExecuteAsync(Guid userId)
    {
        var alerts = await alertRepository.GetByUserAsync(userId);

        return alerts.Select(a => new AlertResponse(
            a.Id,
            a.HealthRecordId,
            a.MetricTypeId,
            a.MetricType.Name,
            a.MetricType.Unit,
            a.HealthRecord.Value,
            a.Severity.ToString(),
            a.Message,
            a.TriggeredAt,
            a.AcknowledgedAt
        )).ToList();

    }

}
