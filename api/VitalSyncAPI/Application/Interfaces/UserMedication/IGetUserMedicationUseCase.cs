using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IGetUserMedicationUseCase
{
    Task<List<UserMedicationResponse>> ExecuteAsync(Guid userId);
}