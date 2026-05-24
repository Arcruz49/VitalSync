using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VitalSyncAPI.Domain.Enums;

namespace VitalSyncAPI.Domain.Entities;

[Table("alerts")]
public class Alert
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("health_record_id")]
    public Guid HealthRecordId { get; set; }

    [Column("metric_type_id")]
    public int MetricTypeId { get; set; }

    [Column("severity")]
    public AlertSeverity Severity { get; set; }

    [Column("message")]
    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    [Column("triggered_at")]
    public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;

    [Column("acknowledged_at")]
    public DateTime? AcknowledgedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [ForeignKey(nameof(HealthRecordId))]
    public HealthRecord HealthRecord { get; set; } = null!;

    [ForeignKey(nameof(MetricTypeId))]
    public MetricType MetricType { get; set; } = null!;
}