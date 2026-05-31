using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IGetAllPersonalRangeUseCase
{
    Task<List<PersonalRangeResponse>> ExecuteAsync(Guid userId);
}