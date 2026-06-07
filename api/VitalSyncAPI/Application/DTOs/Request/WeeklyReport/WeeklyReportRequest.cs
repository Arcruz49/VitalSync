namespace VitalSyncAPI.Application.DTOs.Request;

public record WeeklyReportRequest(
    DateTime? WeekStart
);