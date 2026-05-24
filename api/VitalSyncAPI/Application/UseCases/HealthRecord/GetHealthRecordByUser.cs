using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;

namespace VitalSyncAPI.Application.UseCases;

public class GetHealthRecordByUser(IHealthRecordsRepository recordRepository) : IGetHealthRecordByUser
{
    public async Task<List<HealthRecordResponse>> ExecuteAsync(Guid userId, int? metricTypeId, DateTime? from, DateTime? to)
    {
        var records = await recordRepository.GetByUserAsync(userId, metricTypeId, from, to);

        return records.Select(record => new HealthRecordResponse(
            record.Id,
            record.MetricTypeId,
            record.MetricType.Name,
            record.MetricType.Unit,
            record.Value,
            record.MeasuredAt,
            record.Notes,
            record.CreatedAt
        )).ToList();
    }
}