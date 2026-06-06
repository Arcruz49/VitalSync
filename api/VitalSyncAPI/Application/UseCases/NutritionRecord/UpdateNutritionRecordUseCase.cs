using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Domain.Exceptions;

namespace VitalSyncAPI.Application.UseCases;

public class UpdateNutritionRecordUseCase(
    INutritionRecordRepository recordRepository,
    IUnitOfWork unitOfWork
) : IUpdateNutritionRecordUseCase
{
    public async Task<NutritionResponse> ExecuteAsync(Guid userId, Guid nutritionRecordId, NutritionRequest request)
    {
        var record = await recordRepository.GetById(nutritionRecordId)
            ?? throw new NotFoundException("Registro não encontrado.");

        if (record.UserId != userId)
            throw new ForbiddenException("Você não tem permissão para editar este registro.");

        record.MealType = request.MealType;
        record.Notes = request.Notes;
        record.MeasuredAt = request.MeasuredAt;

        recordRepository.UpdateNutritionRecord(record);
        await unitOfWork.SaveChangesAsync();

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
            record.Status.ToString(),
            record.MeasuredAt,
            record.CreatedAt
        );
    }
}