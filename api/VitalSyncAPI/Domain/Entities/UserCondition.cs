using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VitalSyncAPI.Domain.Entities;

[Table("user_conditions")]
public class UserCondition
{
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("condition_id")]
    public int ConditionId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [ForeignKey(nameof(ConditionId))]
    public HealthCondition Condition { get; set; } = null!;
}