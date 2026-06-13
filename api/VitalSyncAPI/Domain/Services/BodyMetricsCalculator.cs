using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Enums;

namespace VitalSyncAPI.Domain.Services;

public static class BodyMetricsCalculator
{
    public static BodyMetrics Calculate(User user, UserProfile profile, decimal weightKg)
    {
        var age        = CalculateAge(user.BirthDate);
        var isMale     = user.Gender == "M";
        var heightCm   = (double)profile.HeightCm;
        var weight     = (double)weightKg;

        var bmi        = CalculateBMI(weightKg, profile.HeightCm);
        var bmr        = CalculateBMR(weight, heightCm, age, isMale);
        var tdee       = CalculateTDEE(bmr, profile.ActivityLevel);
        var calorieGoal = CalculateCalorieGoal(tdee, profile.Goal);
        var (proteinG, carbsG, fatG) = CalculateMacros(calorieGoal, weightKg, profile.Goal);
        var (idealMin, idealMax)     = CalculateIdealWeight(profile.HeightCm);

        return new BodyMetrics
        {
            Id              = Guid.NewGuid(),
            UserId          = user.Id,
            WeightKg        = weightKg,
            BMI             = bmi,
            IdealWeightMinKg = idealMin,
            IdealWeightMaxKg = idealMax,
            BMR             = (decimal)bmr,
            TDEE            = (decimal)tdee,
            CalorieGoal     = (decimal)calorieGoal,
            ProteinGoalG    = proteinG,
            CarbsGoalG      = carbsG,
            FatGoalG        = fatG,
            RecordedAt      = DateTime.UtcNow
        };
    }


    private static decimal CalculateBMI(decimal weightKg, decimal heightCm)
    {
        var heightM = heightCm / 100;
        return Math.Round(weightKg / (heightM * heightM), 2);
    }

    private static double CalculateBMR(double weightKg, double heightCm, int age, bool isMale)
    {
        var bmr = 10 * weightKg + 6.25 * heightCm - 5 * age;
        return isMale ? bmr + 5 : bmr - 161;
    }

    private static double CalculateTDEE(double bmr, ActivityLevel level)
    {
        var factor = level switch
        {
            ActivityLevel.Sedentary        => 1.2,
            ActivityLevel.LightlyActive    => 1.375,
            ActivityLevel.ModeratelyActive => 1.55,
            ActivityLevel.VeryActive       => 1.725,
            ActivityLevel.ExtremelyActive  => 1.9,
            _                              => 1.2
        };
        return Math.Round(bmr * factor, 2);
    }

    private static double CalculateCalorieGoal(double tdee, HealthGoal goal)
    {
        var calories = goal switch
        {
            HealthGoal.WeightLoss => tdee * 0.80,
            HealthGoal.MuscleGain => tdee + 250,
            HealthGoal.Maintenance => tdee,
            HealthGoal.Conditioning => tdee * 1.05,
            HealthGoal.ChronicConditionControl => tdee * 0.85,
            HealthGoal.GeneralHealth => tdee * 0.90,
            _ => tdee
        };

        return Math.Max(calories, 1200);
    }

    private static (decimal protein, decimal carbs, decimal fat) CalculateMacros(
        double calorieGoal, decimal weightKg, HealthGoal goal)
    {
        // Proteína: 1.8g/kg para ganho de massa, 1.6g/kg para os demais
        var proteinG = goal == HealthGoal.MuscleGain
            ? Math.Round(weightKg * 1.8m, 1)
            : Math.Round(weightKg * 1.6m, 1);

        // Gordura: 25% das calorias totais
        var fatG = Math.Round((decimal)(calorieGoal * 0.25 / 9), 1);

        // Carboidratos: calorias restantes
        var proteinCal = proteinG * 4;
        var fatCal     = fatG * 9;
        var carbsCal   = (decimal)calorieGoal - proteinCal - fatCal;
        var carbsG     = Math.Round(carbsCal / 4, 1);

        return (proteinG, carbsG, fatG);
    }

    private static (decimal min, decimal max) CalculateIdealWeight(decimal heightCm)
    {
        var heightM = heightCm / 100;
        return (
            Math.Round(18.5m * heightM * heightM, 1),
            Math.Round(24.9m * heightM * heightM, 1)
        );
    }

    private static int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.UtcNow;
        var age   = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }
}