namespace VitalSyncAI.Models;

public class InsightResult
{
    public List<string> Insights { get; set; } = [];
    public List<string> Tips { get; set; } = [];
    public string OverallAssessment { get; set; } = string.Empty;
    public string Disclaimer { get; set; } = string.Empty;
}