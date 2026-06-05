using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.Interfaces;

public interface IDeleteNutritionRecordUseCase
{
    Task ExecuteAsync(Guid userId, Guid nutritionRecordId);
}