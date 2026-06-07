using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Domain.Entities;

namespace VitalSyncAPI.Application.Interfaces;

public interface IAddWeeklyReportUseCase
{
    Task<WeeklyReportResponse> ExecuteAsync(Guid userId, WeeklyReportRequest request);
}