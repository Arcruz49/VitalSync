namespace VitalSync.Contracts;

public record WeeklyReportGeneratedEvent(
    Guid ReportId,
    Guid UserId,
    string Summary,
    string MetricsAnalysis,
    string Patterns,
    string Recommendations,
    string? NutritionSummary,
    string Disclaimer,
    bool Success,
    string? ErrorMessage
);