namespace VitalSync.Contracts;

public record WeeklyMetricData(
    string MetricName,
    string Unit,
    decimal Average,
    decimal Min,
    decimal Max,
    decimal FirstValue,
    decimal LastValue,
    int RecordCount,
    string Trend
);