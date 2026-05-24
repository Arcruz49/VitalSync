using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IGetHealthRecordByUser
{
    Task<List<HealthRecordResponse>> ExecuteAsync(Guid userId, int? metricTypeId, DateTime? from, DateTime? to);
}