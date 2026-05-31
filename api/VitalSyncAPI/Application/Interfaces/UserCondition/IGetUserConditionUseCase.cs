using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IGetUserConditionUseCase
{
    Task<List<UserConditionResponse>> ExecuteAsync(Guid userId);
}