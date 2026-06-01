using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VitalSyncAPI.Domain.Entities;

[Table("ai_insights")]
public class AIInsight
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("health_record_id")]
    public Guid? HealthRecordId { get; set; }

    [Column("insights", TypeName = "jsonb")]
    public string Insights { get; set; } = string.Empty;

    [Column("tips", TypeName = "jsonb")]
    public string Tips { get; set; } = string.Empty;

    [Column("overall_assessment")]
    [MaxLength(500)]
    public string OverallAssessment { get; set; } = string.Empty;

    [Column("disclaimer")]
    [MaxLength(300)]
    public string Disclaimer { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [ForeignKey(nameof(HealthRecordId))]
    public HealthRecord? HealthRecord { get; set; }
}