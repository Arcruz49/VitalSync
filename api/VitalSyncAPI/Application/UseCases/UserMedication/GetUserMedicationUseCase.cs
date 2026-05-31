using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.UseCases;

public class GetUserMedicationUseCase(
    IUserMedicationRepository userMedicationRepository
    ) : IGetUserMedicationUseCase
{
    public async Task<List<UserMedicationResponse>> ExecuteAsync(Guid userId)
    {
        var result = await userMedicationRepository.GetByUserId(userId);

        return result.Select(x => new UserMedicationResponse(
            x.Id,
            x.MedicationClass.ToString(),
            x.Notes
        )).ToList();
    }
}