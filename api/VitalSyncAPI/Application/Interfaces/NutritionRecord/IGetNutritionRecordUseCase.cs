using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IGetNutritionRecordUseCase
{
    Task<NutritionResponse> ExecuteAsync(Guid userId, Guid nutritionRecordId);
}