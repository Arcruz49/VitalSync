namespace VitalSync.Contracts;

public record InsightGeneratedEvent(
    Guid HealthRecordId,
    Guid UserId,
    List<string> Insights,
    List<string> Tips,
    string OverallAssessment,
    string Disclaimer
);