namespace VitalSyncAPI.Application.Models;

public class AIAnalysisContext
{
    // Perfil do usuário
    public int Age { get; set; }
    public string Sex { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public string ActivityLevel { get; set; } = string.Empty;
    public List<string> Conditions { get; set; } = [];
    public List<string> Medications { get; set; } = [];

    // Métricas calculadas
    public decimal BMI { get; set; }
    public decimal TDEE { get; set; }
    public decimal CalorieGoal { get; set; }

    // Histórico de registros
    public List<MetricSummary> RecentMetrics { get; set; } = [];

    // Alertas ativos
    public List<string> ActiveAlerts { get; set; } = [];
}

