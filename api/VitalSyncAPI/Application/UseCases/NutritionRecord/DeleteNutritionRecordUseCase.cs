using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using VitalSyncAPI.Domain.Exceptions;
using VitalSyncAPI.Domain.Services;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.UseCases;

public class DeleteNutritionRecordUseCase(
    INutritionRecordRepository recordRepository,
    IUnitOfWork unitOfWork
) : IDeleteNutritionRecordUseCase
{
    public async Task ExecuteAsync(Guid userId, Guid nutritionRecordId)
    {
        var record = await recordRepository.GetById(nutritionRecordId) ?? throw new NotFoundException("Registro não encontrado");

        if(record.UserId != userId) throw new ForbiddenException("Você não tem permissão para deletar este registro.");

        await recordRepository.RemoveNutritionRecord(nutritionRecordId);

        await unitOfWork.SaveChangesAsync();
    }
}


