using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IGetAllUserProfileUseCase
{
    Task<List<UserProfileResponse>> ExecuteAsync(Guid userId);
}