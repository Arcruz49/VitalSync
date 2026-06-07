namespace VitalSync.Contracts;

public class WeeklyReportRawResult
{
    public string Summary { get; set; } = string.Empty;
    public List<WeeklyMetricAnalysisRaw> MetricsAnalysis { get; set; } = [];
    public List<string> Patterns { get; set; } = [];
    public List<string> Recommendations { get; set; } = [];
    public string? NutritionSummary { get; set; }
    public string Disclaimer { get; set; } = string.Empty;
}