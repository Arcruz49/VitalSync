using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VitalSyncAPI.Domain.Entities;

[Table("body_metrics")]
public class BodyMetrics
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("weight_kg", TypeName = "decimal(5,2)")]
    public decimal WeightKg { get; set; }

    // Calculados automaticamente
    [Column("bmi", TypeName = "decimal(4,2)")]
    public decimal BMI { get; set; }

    [Column("ideal_weight_min_kg", TypeName = "decimal(5,2)")]
    public decimal IdealWeightMinKg { get; set; }

    [Column("ideal_weight_max_kg", TypeName = "decimal(5,2)")]
    public decimal IdealWeightMaxKg { get; set; }

    [Column("bmr", TypeName = "decimal(7,2)")]
    public decimal BMR { get; set; } // Taxa Metabólica Basal

    [Column("tdee", TypeName = "decimal(7,2)")]
    public decimal TDEE { get; set; } // Gasto energético total

    [Column("calorie_goal", TypeName = "decimal(7,2)")]
    public decimal CalorieGoal { get; set; } // TDEE ± ajuste por objetivo

    [Column("protein_goal_g", TypeName = "decimal(6,2)")]
    public decimal ProteinGoalG { get; set; }

    [Column("carbs_goal_g", TypeName = "decimal(6,2)")]
    public decimal CarbsGoalG { get; set; }

    [Column("fat_goal_g", TypeName = "decimal(6,2)")]
    public decimal FatGoalG { get; set; }

    [Column("recorded_at")]
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}