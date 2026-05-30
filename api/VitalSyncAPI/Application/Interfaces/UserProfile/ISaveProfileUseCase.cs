using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface ISaveProfileUseCase
{
    Task<UserProfileResponse> ExecuteAsync(Guid userId, UserProfileRequest request);
}