using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface ICreateHealthRecordUseCase
{
    Task ExecuteAsync(Guid userId, HealthRecordRequest request);
}