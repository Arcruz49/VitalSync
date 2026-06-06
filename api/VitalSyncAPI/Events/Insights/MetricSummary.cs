namespace VitalSync.Contracts;

public record MetricSummary(
    string MetricName,
    string Unit,
    decimal LastValue,
    decimal Average
);