using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IAddNutritionRecordUseCase
{
    Task<NutritionResponse> ExecuteAsync(Guid userId, NutritionRequest request);
}