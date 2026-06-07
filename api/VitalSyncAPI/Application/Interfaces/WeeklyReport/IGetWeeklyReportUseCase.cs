using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IGetWeeklyReportUseCase
{
    Task<WeeklyReportResponse> ExecuteAsync(Guid userId, DateTime dateTime);
}