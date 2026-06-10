using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface ISendEmailForgotPasswordUseCase
{
    Task ExecuteAsync(string email);
}