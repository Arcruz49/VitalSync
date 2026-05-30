using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IGetUserProfileUseCase
{
    Task<UserProfileResponse> ExecuteAsync(Guid userId);
}