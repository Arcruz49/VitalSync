using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VitalSyncAPI.Domain.Enums;

namespace VitalSyncAPI.Domain.Entities;

[Table("user_profiles")]
public class UserProfile
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("height_cm")]
    public decimal HeightCm { get; set; }

    [Column("activity_level")]
    public ActivityLevel ActivityLevel { get; set; }

    [Column("goal")]
    public HealthGoal Goal { get; set; }

    [Column("target_weight_kg")]
    public decimal? TargetWeightKg { get; set; }

    [Column("waist_circumference_cm")]
    public decimal? WaistCircumferenceCm { get; set; }

    [Column("body_fat_percent")]
    public decimal? BodyFatPercent { get; set; }

    [Column("training_frequency_days")]
    public int TrainingFrequencyDays { get; set; } // 0–7

    [Column("training_types")]
    public string TrainingTypes { get; set; } = string.Empty;

    [Column("hours_seated_per_day")]
    public HoursSeated HoursSeated { get; set; }

    [Column("habitual_sleep_hours")]
    public decimal HabitualSleepHours { get; set; }

    [Column("sleep_quality")]
    public int SleepQuality { get; set; } // 1–5

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}