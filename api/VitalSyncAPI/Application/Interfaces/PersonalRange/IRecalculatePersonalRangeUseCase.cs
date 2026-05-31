using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IRecalculatePersonalRangeUseCase
{
    Task ExecuteAsync(Guid userId);
}