namespace VitalSyncAPI.Application.DTOs.Responses;

public record PersonalRangeResponse(
    Guid Id,
    int MetricTypeId,
    string MetricTypeName,
    string Unit,
    decimal MinNormal,
    decimal MaxNormal,
    string Method,
    DateTime CalculatedAt
);