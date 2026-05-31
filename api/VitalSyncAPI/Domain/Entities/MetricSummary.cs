namespace VitalSyncAPI.Application.Models;
public class MetricSummary
{
    public string MetricName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal LastValue { get; set; }
    public decimal Average { get; set; }
    public string Trend { get; set; } = string.Empty; // "improving", "stable", "worsening"
    public List<decimal> Last7Values { get; set; } = [];
}