namespace VitalSyncAPI.Application.DTOs.Responses;

public record HealthRecordResponse(
    Guid Id,
    int MetricTypeId,
    string MetricTypeName,
    string Unit,
    decimal Value,
    DateTime MeasuredAt,
    string? Notes,
    DateTime CreatedAt
);