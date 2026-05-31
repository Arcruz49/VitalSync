using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace VitalSyncAPI.Application.UseCases;

public class AddUserMedicationUseCase(
    IUserMedicationRepository userMedicationRepository,
    IUnitOfWork unitOfWork
    ) : IAddUserMedicationUseCase
{
    public async Task ExecuteAsync(Guid userId, List<UserMedicationRequest> request)
    {
        var hasDuplicates = request
            .GroupBy(x => x.MedicationClass)
            .Any(g => g.Count() > 1);

        if (hasDuplicates) throw new ValidationException("A lista contém medicamentos duplicados.");

        await userMedicationRepository.RemoveAllByUserId(userId);

        foreach (var medication in request)
        {
            await userMedicationRepository.AddMedication(new UserMedication
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                MedicationClass = medication.MedicationClass,
                Notes = medication.Notes,
            });
        }

        await unitOfWork.SaveChangesAsync();
    }
}