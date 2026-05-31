using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IAddUserConditionUseCase
{
    Task ExecuteAsync(Guid userId, List<UserConditionRequest> request);
}