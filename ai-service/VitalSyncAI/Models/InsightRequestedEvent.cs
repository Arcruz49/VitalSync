namespace VitalSync.Contracts;

public record InsightRequestedEvent(
    Guid HealthRecordId,
    Guid UserId,
    string MetricTypeName,
    string Unit,
    decimal Value,
    DateTime MeasuredAt,
    string Goal,
    string ActivityLevel,
    decimal BMI,
    decimal TDEE,
    decimal CalorieGoal,
    List<string> Conditions,
    List<string> Medications,
    List<MetricSummary> RecentMetrics
);

