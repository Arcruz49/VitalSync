namespace VitalSyncAI.Models;

public class WeeklyReportResult
{
    public string Summary { get; set; } = string.Empty;
    public string MetricsAnalysis { get; set; } = "[]";
    public string Patterns { get; set; } = "[]";
    public string Recommendations { get; set; } = "[]";
    public string? NutritionSummary { get; set; }
    public string Disclaimer { get; set; } = string.Empty;
}