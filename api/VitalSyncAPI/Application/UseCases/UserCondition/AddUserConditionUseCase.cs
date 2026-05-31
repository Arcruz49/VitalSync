using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace VitalSyncAPI.Application.UseCases;

public class AddUserConditionUseCase(
    IUserConditionRepository userConditionRepository,
    IUnitOfWork unitOfWork
    ) : IAddUserConditionUseCase
{
    public async Task ExecuteAsync(Guid userId, List<UserConditionRequest> request)
    {
        await userConditionRepository.RemoveAllByUserId(userId);
        
        foreach (var condition in request)
        {
            var exists = await userConditionRepository.GetByUserIdAndConditionId(userId, condition.ConditionId);

            if (exists is not null) throw new ValidationException($"Condição {condition.ConditionId} já cadastrada.");

            await userConditionRepository.AddCondition(new UserCondition
            {
                UserId = userId,
                ConditionId = condition.ConditionId
            });
        }

        await unitOfWork.SaveChangesAsync();
    }
}