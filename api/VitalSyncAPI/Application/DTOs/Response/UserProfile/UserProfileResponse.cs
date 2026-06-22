namespace VitalSyncAPI.Application.DTOs.Responses;

public record UserProfileResponse(
    Guid Id,
    Guid UserId,

    decimal HeightCm,
    decimal WeightKg,
    decimal? TargetWeightKg,
    decimal? WaistCircumferenceCm,
    decimal? BodyFatPercent,

    string ActivityLevel,
    string Goal,
    int TrainingFrequencyDays,
    List<string> TrainingTypes,
    string HoursSeated,
    string ProfilePic,
    decimal HabitualSleepHours,
    int SleepQuality,

    decimal BMI,
    decimal IdealWeightMinKg,
    decimal IdealWeightMaxKg,
    decimal BMR,
    decimal TDEE,
    decimal CalorieGoal,
    decimal ProteinGoalG,
    decimal CarbsGoalG,
    decimal FatGoalG,

    DateTime CreatedAt,
    DateTime UpdatedAt
);