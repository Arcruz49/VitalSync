using VitalSyncAPI.Domain.Entities;

namespace VitalSyncAPI.Domain.Interfaces;

public interface INutritionRecordRepository
{
    Task<List<NutritionRecord>> GetByUserId(Guid userId, DateTime? from, DateTime? to);
    Task<NutritionRecord?> GetById(Guid nutritionId);
    Task AddNutritionRecord(NutritionRecord nutritionRecord);
    void UpdateNutritionRecord(NutritionRecord nutritionRecord);
    Task RemoveNutritionRecord(Guid nutritionId);
}