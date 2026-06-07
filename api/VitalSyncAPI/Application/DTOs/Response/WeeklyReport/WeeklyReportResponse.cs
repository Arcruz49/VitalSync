namespace VitalSyncAPI.Application.DTOs.Responses;

public record WeeklyReportResponse(
    Guid Id,
    DateTime WeekStart,
    DateTime WeekEnd,
    string Summary,
    List<string> Patterns,
    List<string> Recommendations,
    string? NutritionSummary,
    string Disclaimer,
    string Status,
    DateTime CreatedAt,
    List<WeeklyMetricAnalysisResponse>? MetricsAnalysis
);