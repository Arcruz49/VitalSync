namespace VitalSyncAPI.Application.DTOs.Request;

public record HealthRecordRequest(
    int MetricTypeId,
    decimal Value,
    DateTime MeasuredAt,
    string? Notes
);