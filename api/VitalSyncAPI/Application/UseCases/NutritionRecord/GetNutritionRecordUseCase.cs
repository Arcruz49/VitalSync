using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using VitalSyncAPI.Domain.Exceptions;
using VitalSyncAPI.Domain.Services;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.UseCases;

public class GetNutritionRecordUseCase(
    INutritionRecordRepository recordRepository
) : IGetNutritionRecordUseCase
{
    public async Task<NutritionResponse> ExecuteAsync(Guid userId, Guid nutritionRecordId)
    {
        var record = await recordRepository.GetById(nutritionRecordId) ?? throw new NotFoundException("Registro não encontrado");

        if (record.UserId != userId)
            throw new ForbiddenException("Você não tem permissão para editar este registro.");

        return new NutritionResponse(
            record.Id,
            record.MealType.ToString(),
            record.FoodDescription,
            record.CaloriesKcal,
            record.ProteinG,
            record.CarbsG,
            record.FatG,
            record.Confidence,
            record.Notes,
            record.MeasuredAt,
            record.CreatedAt
        );
    }
}


