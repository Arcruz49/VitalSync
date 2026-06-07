using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IGetWeeklyReportsUseCase
{
    Task<List<WeeklyReportResponse>> ExecuteAsync(Guid userId);
}