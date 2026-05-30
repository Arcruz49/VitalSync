using VitalSyncAPI.Domain.Entities;

namespace VitalSyncAPI.Domain.Interfaces;


public interface IUserConditionRepository
{
    Task<List<UserCondition>> GetByUserId(Guid userId);
    Task<UserCondition?> GetByUserIdAndConditionId(Guid userId, int conditionId);
    Task AddCondition(UserCondition userCondition);
    Task RemoveCondition(Guid userId, int conditionId);
}