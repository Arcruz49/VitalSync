using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IGetWeeklyReportsByIdUseCase
{
    Task<WeeklyReportResponse> ExecuteAsync(Guid id, Guid userId);
}