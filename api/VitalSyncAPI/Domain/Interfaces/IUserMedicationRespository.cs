using VitalSyncAPI.Domain.Entities;

namespace VitalSyncAPI.Domain.Interfaces;


public interface IUserMedicationRespository
{
    Task<List<UserMedication>> GetByUserId(Guid userId);
    Task<UserMedication?> GetByUserIdAndMedicationId(Guid userId, int medicationId);
    Task AddCondition(UserCondition userCondition);
    Task RemoveCondition(Guid userId, int medicationId);
    Task RemoveAllByUserId(Guid userId);
}