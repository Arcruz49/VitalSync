namespace VitalSyncAPI.Application.DTOs.Responses;

public record NutritionSummaryResponse(
    decimal TotalCaloriesKcal,
    decimal TotalProteinG,
    decimal TotalCarbsG,
    decimal TotalFatG,
    decimal CalorieGoal,
    decimal ProteinGoalG,
    decimal CarbsGoalG,
    decimal FatGoalG,
    List<NutritionResponse> Records
);