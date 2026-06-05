namespace VitalSyncAPI.Application.Models;

public class NutritionAnalysisResult
{
    public string FoodDescription { get; set; } = string.Empty;
    public decimal CaloriesKcal { get; set; }
    public decimal ProteinG { get; set; }
    public decimal CarbsG { get; set; }
    public decimal FatG { get; set; }
    public decimal Confidence { get; set; }
    public string Disclaimer { get; set; } = string.Empty;
}