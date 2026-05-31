using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using VitalSyncAPI.Domain.Exceptions;
using VitalSyncAPI.Domain.Services;

namespace VitalSyncAPI.Application.UseCases;

public class RecalculatePersonalRangeUseCase(
    IUserProfileRepository userProfileRepository,
    IUserConditionRepository userConditionRepository,
    IUserMedicationRepository userMedicationRepository,
    IPersonalRangeRepository personalRangeRepository,
    IMetricTypesRepository metricTypeRepository,
    IUnitOfWork unitOfWork) : IRecalculatePersonalRangeUseCase
{
    public async Task ExecuteAsync(Guid userId)
    {
        var profile = await userProfileRepository.GetByUserId(userId) ?? throw new NotFoundException("Perfil não encontrado.");
        var conditions = await userConditionRepository.GetByUserId(userId);
        var medications = await userMedicationRepository.GetByUserId(userId);
        var metricTypes = await metricTypeRepository.GetAll();

        var ranges = PersonalRangeCalculator.Calculate(userId, profile, conditions, medications, metricTypes);

        await personalRangeRepository.RemoveAllByUserId(userId);
        await personalRangeRepository.AddPersonalRanges(ranges);
        await unitOfWork.SaveChangesAsync();
    }
}