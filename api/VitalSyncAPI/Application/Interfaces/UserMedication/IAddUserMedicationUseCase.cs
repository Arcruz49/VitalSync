using VitalSyncAPI.Application.DTOs.Request;

namespace VitalSyncAPI.Application.Interfaces;

public interface IAddUserMedicationUseCase
{
    Task ExecuteAsync(Guid userId, List<UserMedicationRequest> request);
}