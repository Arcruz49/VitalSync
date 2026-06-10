using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IResetPasswordUseCase
{
    Task ExecuteAsync(string token, string password);
}