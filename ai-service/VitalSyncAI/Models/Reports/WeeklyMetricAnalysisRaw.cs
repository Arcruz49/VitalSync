namespace VitalSync.Contracts;

public class WeeklyMetricAnalysisRaw
{
    public string MetricName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Average { get; set; }
    public decimal Min { get; set; }
    public decimal Max { get; set; }
    public string Trend { get; set; } = string.Empty;
    public string Analysis { get; set; } = string.Empty;
}
