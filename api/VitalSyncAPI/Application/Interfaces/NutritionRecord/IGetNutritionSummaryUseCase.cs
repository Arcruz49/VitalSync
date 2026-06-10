using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IGetNutritionSummaryUseCase
{
    Task<NutritionSummaryResponse> ExecuteAsync(Guid userId, DateTime date, int timezoneOffsetMinutes = 0);
}