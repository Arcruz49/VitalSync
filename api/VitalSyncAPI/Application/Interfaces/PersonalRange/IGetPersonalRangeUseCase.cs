using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IGetPersonalRangeUseCase
{
    Task<PersonalRangeResponse> ExecuteAsync(Guid userId, int metricTypeId);
}