using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IRegisterUserUseCase
{
    Task<UserDto> ExecuteAsync(RegisterUserRequest request);
}