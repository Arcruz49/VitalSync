namespace VitalSyncAPI.Application.DTOs.Responses;

public record AlertResponse(
    Guid Id,
    Guid HealthRecordId,
    int MetricTypeId,
    string MetricTypeName,
    string Unit,
    decimal Value,
    string Severity,
    string Message,
    DateTime TriggeredAt,
    DateTime? AcknowledgedAt
);
