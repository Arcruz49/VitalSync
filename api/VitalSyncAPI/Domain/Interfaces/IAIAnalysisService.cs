using VitalSyncAPI.Application.Models;
using VitalSyncAPI.Domain.Entities;

namespace VitalSyncAPI.Domain.Interfaces;

public interface IAIAnalysisService
{
    Task<AIAnalysisResult> AnalyzeAsync(UserProfile profile, BodyMetrics metrics, List<HealthRecord> records);
    Task<NutritionAnalysisResult> AnalyzeFoodImageAsync(string imageBase64);
}