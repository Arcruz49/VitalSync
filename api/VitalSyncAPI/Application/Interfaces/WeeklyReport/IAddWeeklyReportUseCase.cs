using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IAddWeeklyReportUseCase
{
    Task<List<WeeklyReportResponse>> ExecuteAsync(Guid userId);
}