using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IUpdateNutritionRecordUseCase
{
    Task<NutritionResponse> ExecuteAsync(Guid userId, Guid nutritionRecordId, NutritionRequest request);
}