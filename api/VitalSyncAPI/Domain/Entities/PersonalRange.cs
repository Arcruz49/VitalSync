using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VitalSyncAPI.Domain.Enums;

namespace VitalSyncAPI.Domain.Entities;

[Table("personal_ranges")]
public class PersonalRange
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("metric_type_id")]
    public int MetricTypeId { get; set; }

    [Column("min_normal", TypeName = "decimal(8,2)")]
    public decimal MinNormal { get; set; }

    [Column("max_normal", TypeName = "decimal(8,2)")]
    public decimal MaxNormal { get; set; }

    [Column("method")]
    public RangeMethod Method { get; set; }

    [Column("calculated_at")]
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [ForeignKey(nameof(MetricTypeId))]
    public MetricType MetricType { get; set; } = null!;
}