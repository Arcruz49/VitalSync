using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VitalSyncAPI.Domain.Enums;

namespace VitalSyncAPI.Domain.Entities;

[Table("nutrition_records")]
public class NutritionRecord
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("meal_type")]
    public MealType MealType { get; set; }

    [Column("food_description")]
    [MaxLength(500)]
    public string FoodDescription { get; set; } = string.Empty;

    [Column("calories_kcal", TypeName = "decimal(8,2)")]
    public decimal CaloriesKcal { get; set; }

    [Column("protein_g", TypeName = "decimal(8,2)")]
    public decimal ProteinG { get; set; }

    [Column("carbs_g", TypeName = "decimal(8,2)")]
    public decimal CarbsG { get; set; }

    [Column("fat_g", TypeName = "decimal(8,2)")]
    public decimal FatG { get; set; }

    [Column("confidence", TypeName = "decimal(3,2)")]
    public decimal Confidence { get; set; }

    [Column("notes")]
    [MaxLength(300)]
    public string? Notes { get; set; }

    [Column("measured_at")]
    public DateTime MeasuredAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}