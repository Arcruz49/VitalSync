namespace VitalSync.Contracts;

public record NutritionAnalysisRequestedEvent(
    Guid NutritionRecordId,
    Guid UserId,
    string ImageBase64,
    string MealType
);