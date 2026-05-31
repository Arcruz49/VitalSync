using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.UseCases;

public class GetUserConditionUseCase(
    IUserConditionRepository userConditionRepository
    ) : IGetUserConditionUseCase
{
    public async Task<List<UserConditionResponse>> ExecuteAsync(Guid userId)
    {
        var result = await userConditionRepository.GetByUserId(userId);

        return result.Select(x => new UserConditionResponse(
            x.ConditionId,
            x.Condition.Name
        )).ToList();
    }
}