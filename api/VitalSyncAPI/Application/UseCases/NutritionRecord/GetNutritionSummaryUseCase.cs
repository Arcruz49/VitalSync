using VitalSyncAPI.Application.Interfaces;
using VitalSyncAPI.Domain.Interfaces;
using VitalSyncAPI.Application.DTOs.Responses;

namespace VitalSyncAPI.Application.UseCases;

public class GetNutritionSummaryUseCase(
    INutritionRecordRepository nutritionRecordRepository,
    IBodyMetricsRepository bodyMetricsRepository
) : IGetNutritionSummaryUseCase
{
    public async Task<NutritionSummaryResponse> ExecuteAsync(Guid userId, DateTime date, int timezoneOffsetMinutes = 0)
    {
        // Convert local midnight to UTC: local 00:00 + offset = UTC equivalent.
        // JS getTimezoneOffset() is positive for zones behind UTC (e.g. BRT UTC-3 → 180).
        var localMidnightUtc = date.Date.AddMinutes(timezoneOffsetMinutes);
        var from = DateTime.SpecifyKind(localMidnightUtc, DateTimeKind.Utc);
        var to = DateTime.SpecifyKind(localMidnightUtc.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

        var records = await nutritionRecordRepository.GetByUserId(userId, from, to);
        var metrics = await bodyMetricsRepository.GetLatestByUserId(userId);

        var totalCalories = records.Sum(x => x.CaloriesKcal);
        var totalProtein  = records.Sum(x => x.ProteinG);
        var totalCarbs    = records.Sum(x => x.CarbsG);
        var totalFat      = records.Sum(x => x.FatG);

        var nutritionResponses = records.Select(x => new NutritionResponse(
            x.Id,
            x.MealType.ToString(),
            x.FoodDescription,
            x.CaloriesKcal,
            x.ProteinG,
            x.CarbsG,
            x.FatG,
            x.Confidence,
            x.Notes,
            x.Status.ToString(),
            x.MeasuredAt,
            x.CreatedAt
        )).ToList();

        return new NutritionSummaryResponse(
            totalCalories,
            totalProtein,
            totalCarbs,
            totalFat,
            metrics?.CalorieGoal ?? 0,
            metrics?.ProteinGoalG ?? 0,
            metrics?.CarbsGoalG ?? 0,
            metrics?.FatGoalG ?? 0,
            nutritionResponses
        );
    }
}