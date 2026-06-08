using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VitalSyncAPI.Domain.Enums;

namespace VitalSyncAPI.Domain.Entities;

[Table("weekly_reports")]
public class WeeklyReport
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("week_start")]
    public DateTime WeekStart { get; set; }

    [Column("week_end")]
    public DateTime WeekEnd { get; set; }

    [Column("summary")]
    [MaxLength(1000)]
    public string Summary { get; set; } = string.Empty;

    [Column("metrics_analysis", TypeName = "jsonb")]
    public string MetricsAnalysis { get; set; } = "null";

    [Column("patterns", TypeName = "jsonb")]
    public string Patterns { get; set; } = "[]";

    [Column("recommendations", TypeName = "jsonb")]
    public string Recommendations { get; set; } = "[]";

    [Column("nutrition_summary")]
    [MaxLength(500)]
    public string? NutritionSummary { get; set; }

    [Column("disclaimer")]
    [MaxLength(300)]
    public string Disclaimer { get; set; } = string.Empty;

    [Column("status")]
    public ReportStatus Status { get; set; } = ReportStatus.Pending;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
