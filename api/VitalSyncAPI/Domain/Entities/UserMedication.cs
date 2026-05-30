using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VitalSyncAPI.Domain.Enums;

namespace VitalSyncAPI.Domain.Entities;

[Table("user_medications")]
public class UserMedication
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("medication_class")]
    public MedicationClass MedicationClass { get; set; }

    [Column("notes")]
    [MaxLength(200)]
    public string? Notes { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}