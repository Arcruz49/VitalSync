using VitalSyncAPI.Domain.Enums;

namespace VitalSyncAPI.Application.DTOs.Request;

public record NutritionRequest(
    MealType MealType,
    string? Notes,
    DateTime MeasuredAt,
    string ImageBase64
);