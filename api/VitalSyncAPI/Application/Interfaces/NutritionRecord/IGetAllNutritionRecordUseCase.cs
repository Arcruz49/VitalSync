using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IGetAllNutritionRecordUseCase
{
    Task<List<NutritionResponse>> ExecuteAsync(Guid userId, DateTime? from, DateTime? to);
}