namespace VitalSyncAPI.Application.DTOs.Responses;

public record WeeklyMetricAnalysisResponse(
    string MetricName,
    string Unit,
    decimal Average,
    decimal Min,
    decimal Max,
    string Trend
);