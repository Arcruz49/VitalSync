namespace VitalSyncAPI.Application.DTOs.Responses;

public record MetricTypeResponse(
    int Id,
    string Name,
    string Unit,
    double? MinNormal,
    double? MaxNormal,
    string Icon,
    int SortOrder
);