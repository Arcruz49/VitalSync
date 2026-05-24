using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IEditHealthRecordUseCase
{
    Task ExecuteAsync(Guid userId, Guid id, HealthRecordRequest request);
}