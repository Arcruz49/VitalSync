using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface ICreateHealthRecordUseCase
{
    Task<HealthRecordResponse> ExecuteAsync(Guid userId, HealthRecordRequest request);
}