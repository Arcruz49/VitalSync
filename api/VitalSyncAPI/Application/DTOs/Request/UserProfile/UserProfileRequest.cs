using VitalSyncAPI.Domain.Enums;

namespace VitalSyncAPI.Application.DTOs.Request;

public record UserProfileRequest(
    decimal HeightCm,
    decimal WeightKg,
    decimal? TargetWeightKg,
    decimal? WaistCircumferenceCm,
    decimal? BodyFatPercent,

    ActivityLevel ActivityLevel,
    HealthGoal Goal,
    int TrainingFrequencyDays,        
    List<string> TrainingTypes,       
    HoursSeated HoursSeated,
    decimal HabitualSleepHours,
    int SleepQuality,                  
    ProfilePic ProfilePic                 
);