namespace VitalSync.Contracts;

public record NutritionAnalysisCompletedEvent(
    Guid NutritionRecordId,
    Guid UserId,
    string FoodDescription,
    decimal CaloriesKcal,
    decimal ProteinG,
    decimal CarbsG,
    decimal FatG,
    decimal Confidence,
    bool Success,
    string? ErrorMessage
);