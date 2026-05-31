using Microsoft.EntityFrameworkCore;
using VitalSyncAPI.Infrastructure.Data;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Domain.Exceptions;

namespace VitalSyncAPI.Infrastructure.Repositories;

public class UserMedicationRespository(Context db) : BaseRepository<UserCondition>(db), IUserMedicationRespository
{
    public async Task<List<UserCondition>> GetByUserId(Guid userId)
    {
        return await Query()
            .Include(x => x.Condition)
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserCondition?> GetByUserIdAndConditionId(Guid userId, int conditionId)
    {
        return await Query()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ConditionId == conditionId);
    }

    public async Task AddCondition(UserCondition userCondition)
    {
        await AddAsync(userCondition);
    }

    public async Task RemoveCondition(Guid userId, int conditionId)
    {
        var condition = await _db.UserConditions
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ConditionId == conditionId)
            ?? throw new NotFoundException("Condição não encontrada.");

        _db.UserConditions.Remove(condition);
    }
    public async Task RemoveAllByUserId(Guid userId)
    {
        var conditions = await _db.UserConditions
            .Where(x => x.UserId == userId)
            .ToListAsync();

        _db.UserConditions.RemoveRange(conditions);
    }
}