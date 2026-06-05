using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.UseCases;

public class AddNutritionRecordUseCase(
    INutritionRecordRepository recordRepository,
    IAIAnalysisService aiAnalysisService,
    IUnitOfWork unitOfWork
) : IAddNutritionRecordUseCase
{
    public async Task<NutritionResponse> ExecuteAsync(Guid userId, NutritionRequest request)
    {
        var analysis = await aiAnalysisService.AnalyzeFoodImageAsync(request.ImageBase64);

        var record = new NutritionRecord
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            MealType = request.MealType,
            FoodDescription = analysis.FoodDescription,
            CaloriesKcal = analysis.CaloriesKcal,
            ProteinG = analysis.ProteinG,
            CarbsG = analysis.CarbsG,
            FatG = analysis.FatG,
            Confidence = analysis.Confidence,
            Notes = request.Notes,
            MeasuredAt = request.MeasuredAt,
            CreatedAt = DateTime.UtcNow
        };

        await recordRepository.AddNutritionRecord(record);
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
            record.MeasuredAt,
            record.CreatedAt
        );
    }
}