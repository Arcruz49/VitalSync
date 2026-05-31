using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Enums;

namespace VitalSyncAPI.Domain.Interfaces;


public interface IUserMedicationRepository
{
    Task<List<UserMedication>> GetByUserId(Guid userId);
    Task<UserMedication?> GetByUserIdAndMedicationId(Guid userId, MedicationClass medicationClass);
    Task AddMedication(UserMedication userMedication);
    Task RemoveMedication(Guid userId, MedicationClass medicationClass);
    Task RemoveAllByUserId(Guid userId);
}