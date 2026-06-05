using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using VitalSyncAPI.Domain.Exceptions;
using VitalSyncAPI.Domain.Services;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.UseCases;

public class GetAllNutritionRecordUseCase(
    INutritionRecordRepository recordRepository
) : IGetAllNutritionRecordUseCase
{
    public async Task<List<NutritionResponse>> ExecuteAsync(Guid userId, DateTime? from, DateTime? to)
    {
        var record = await recordRepository.GetByUserId(userId, from, to);

        return record.Select(x => new NutritionResponse(
            x.Id,
            x.MealType.ToString(),
            x.FoodDescription,
            x.CaloriesKcal,
            x.ProteinG,
            x.CarbsG,
            x.FatG,
            x.Confidence,
            x.Notes,
            x.MeasuredAt,
            x.CreatedAt
        )).ToList();
    }
}


