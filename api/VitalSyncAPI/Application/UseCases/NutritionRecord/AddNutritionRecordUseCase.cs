using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Application.DTOs.Responses;
using VitalSyncAPI.Domain.Enums;
using VitalSync.Contracts;
using MassTransit;

namespace VitalSyncAPI.Application.UseCases;

public class AddNutritionRecordUseCase(
    INutritionRecordRepository recordRepository,
    IUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint
) : IAddNutritionRecordUseCase
{
    public async Task<NutritionResponse> ExecuteAsync(Guid userId, NutritionRequest request)
    {
        var record = new NutritionRecord
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            MealType = request.MealType,
            FoodDescription = "Analisando...",
            CaloriesKcal = 0,
            ProteinG = 0,
            CarbsG = 0,
            FatG = 0,
            Confidence = 0,
            Status = NutritionStatus.Pending,
            Notes = request.Notes,
            MeasuredAt = request.MeasuredAt,
            CreatedAt = DateTime.UtcNow
        };

        await recordRepository.AddNutritionRecord(record);
        await unitOfWork.SaveChangesAsync();

        await publishEndpoint.Publish(new NutritionAnalysisRequestedEvent(
            record.Id,
            userId,
            request.ImageBase64,
            request.MealType.ToString()
        ));

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