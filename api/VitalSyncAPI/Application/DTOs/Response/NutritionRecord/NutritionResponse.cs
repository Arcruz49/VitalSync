namespace VitalSyncAPI.Application.DTOs.Responses;

public record NutritionResponse(
    Guid Id,
    string MealType,
    string FoodDescription,
    decimal CaloriesKcal,
    decimal ProteinG,
    decimal CarbsG,
    decimal FatG,
    decimal Confidence,
    string? Notes,
    DateTime MeasuredAt,
    DateTime CreatedAt
);