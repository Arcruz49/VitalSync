using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Domain.Enums;
using System.Text.Json;
using VitalSyncAPI.Domain.Services;
using VitalSyncAPI.Domain.Exceptions;

namespace VitalSyncAPI.Application.UseCases;

public class GetAllUserProfileUseCase(
    IUserProfileRepository userProfileRepository,
    IBodyMetricsRepository bodyMetricsRepository
    ) : IGetAllUserProfileUseCase
{
    public async Task<List<UserProfileResponse>> ExecuteAsync(Guid userId)
    {
        var profile = await userProfileRepository.GetByUserId(userId)
            ?? throw new NotFoundException("Perfil não encontrado.");

        var allMetrics = await bodyMetricsRepository.GetAllByUserId(userId);

        if (allMetrics.Count == 0)
            throw new NotFoundException("Métricas não encontradas.");

        return allMetrics.Select(metrics => new UserProfileResponse(
            profile.Id,
            profile.UserId,
            profile.HeightCm,
            metrics.WeightKg,
            profile.TargetWeightKg,
            profile.WaistCircumferenceCm,
            profile.BodyFatPercent,
            profile.ActivityLevel.ToString(),
            profile.Goal.ToString(),
            profile.TrainingFrequencyDays,
            JsonSerializer.Deserialize<List<string>>(profile.TrainingTypes) ?? [],
            profile.HoursSeated.ToString(),
            profile.HabitualSleepHours,
            profile.SleepQuality,
            metrics.BMI,
            metrics.IdealWeightMinKg,
            metrics.IdealWeightMaxKg,
            metrics.BMR,
            metrics.TDEE,
            metrics.CalorieGoal,
            metrics.ProteinGoalG,
            metrics.CarbsGoalG,
            metrics.FatGoalG,
            profile.CreatedAt,
            profile.UpdatedAt
        )).ToList();
    }
}