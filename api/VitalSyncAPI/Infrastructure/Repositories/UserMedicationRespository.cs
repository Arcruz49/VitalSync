using Microsoft.EntityFrameworkCore;
using VitalSyncAPI.Infrastructure.Data;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Domain.Exceptions;
using VitalSyncAPI.Domain.Enums;

namespace VitalSyncAPI.Infrastructure.Repositories;

public class UserMedicationRepository(Context db) : BaseRepository<UserMedication>(db), IUserMedicationRepository
{
    public async Task<List<UserMedication>> GetByUserId(Guid userId)
    {
        return await Query()
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserMedication?> GetByUserIdAndMedicationId(Guid userId, MedicationClass medicationClass)
    {
        return await Query()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.MedicationClass == medicationClass);
    }

    public async Task AddMedication(UserMedication userMedication)
    {
        await AddAsync(userMedication);
    }

    public async Task RemoveMedication(Guid userId, MedicationClass medicationClass)
    {
        var medication = await _db.UserMedications
            .FirstOrDefaultAsync(x => x.UserId == userId && x.MedicationClass == medicationClass)
            ?? throw new NotFoundException("Medicamento não encontrado.");

        _db.UserMedications.Remove(medication);
    }

    public async Task RemoveAllByUserId(Guid userId)
    {
        var medications = await _db.UserMedications
            .Where(x => x.UserId == userId)
            .ToListAsync();

        _db.UserMedications.RemoveRange(medications);
    }
}