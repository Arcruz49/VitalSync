using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IGetHealthRecordById
{
    Task<HealthRecordResponse> ExecuteAsync(Guid userId, Guid id);
}