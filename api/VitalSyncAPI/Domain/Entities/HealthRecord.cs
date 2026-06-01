using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VitalSyncAPI.Domain.Enums;

namespace VitalSyncAPI.Domain.Entities;

[Table("health_records")]
public class HealthRecord
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("metric_type_id")]
    public int MetricTypeId { get; set; }

    [Column("value", TypeName = "decimal(8,2)")]
    public decimal Value { get; set; }

    [Column("measured_at")]
    public DateTime MeasuredAt { get; set; }

    [Column("notes")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    [Column("source")]
    public RecordSource Source { get; set; } = RecordSource.Manual;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [ForeignKey(nameof(MetricTypeId))]
    public MetricType MetricType { get; set; } = null!;
}