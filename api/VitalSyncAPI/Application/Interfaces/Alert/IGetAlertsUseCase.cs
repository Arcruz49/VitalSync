using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IGetAlertsUseCase
{
    Task<List<AlertResponse>> ExecuteAsync(Guid userId);
}