using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IAuthenticateUseCase
{
    Task<UserDto> ExecuteAsync(LoginRequest request);
}