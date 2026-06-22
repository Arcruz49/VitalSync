using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Domain.Enums;
using System.Text.Json;
using VitalSyncAPI.Domain.Services;

namespace VitalSyncAPI.Application.UseCases;

public class SaveProfileUseCase(
    IUserRepository userRepository,
    IUserProfileRepository userProfileRepository,
    IBodyMetricsRepository bodyMetricsRepository,
    IUnitOfWork unitOfWork) : ISaveProfileUseCase
{
    public async Task<UserProfileResponse> ExecuteAsync(Guid userId, UserProfileRequest request)
    {
        var user = await userRepository.GetUserById(userId);
        var existingProfile = await userProfileRepository.GetByUserId(userId);

        UserProfile profile;

        if (existingProfile is null)
        {
            profile = new UserProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                HeightCm = request.HeightCm,
                ActivityLevel = request.ActivityLevel,
                Goal = request.Goal,
                TargetWeightKg = request.TargetWeightKg,
                WaistCircumferenceCm = request.WaistCircumferenceCm,
                BodyFatPercent = request.BodyFatPercent,
                TrainingFrequencyDays = request.TrainingFrequencyDays,
                TrainingTypes = JsonSerializer.Serialize(request.TrainingTypes),
                HoursSeated = request.HoursSeated,
                HabitualSleepHours = request.HabitualSleepHours,
                SleepQuality = request.SleepQuality,
                ProfilePic = request.ProfilePic,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await userProfileRepository.CreateProfile(profile);
        }
        else
        {
            existingProfile.HeightCm = request.HeightCm;
            existingProfile.ActivityLevel = request.ActivityLevel;
            existingProfile.Goal = request.Goal;
            existingProfile.TargetWeightKg = request.TargetWeightKg;
            existingProfile.WaistCircumferenceCm = request.WaistCircumferenceCm;
            existingProfile.BodyFatPercent = request.BodyFatPercent;
            existingProfile.TrainingFrequencyDays = request.TrainingFrequencyDays;
            existingProfile.TrainingTypes = JsonSerializer.Serialize(request.TrainingTypes);
            existingProfile.HoursSeated = request.HoursSeated;
            existingProfile.HabitualSleepHours = request.HabitualSleepHours;
            existingProfile.SleepQuality = request.SleepQuality;
            existingProfile.ProfilePic = request.ProfilePic;
            existingProfile.UpdatedAt = DateTime.UtcNow;

            userProfileRepository.UpdateProfile(existingProfile);
            profile = existingProfile;
        }

        var metrics = BodyMetricsCalculator.Calculate(user, profile, request.WeightKg);
        await bodyMetricsRepository.CreateMetrics(metrics);

        await unitOfWork.SaveChangesAsync();

        return new UserProfileResponse(
            profile.Id,
            profile.UserId,
            profile.HeightCm,
            request.WeightKg,
            profile.TargetWeightKg,
            profile.WaistCircumferenceCm,
            profile.BodyFatPercent,
            profile.ActivityLevel.ToString(),
            profile.Goal.ToString(),
            profile.TrainingFrequencyDays,
            JsonSerializer.Deserialize<List<string>>(profile.TrainingTypes) ?? [],
            profile.HoursSeated.ToString(),
            profile.ProfilePic.ToString(),
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
        );
    }
}