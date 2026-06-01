using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VitalSyncAPI.Domain.Entities;

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("password")]
    public string Password { get; set; } = string.Empty;

    [Column("gender")]
    public string Gender { get; set; } = string.Empty;

    [Column("birthdate")]
    public DateTime BirthDate { get; set; }

    [Column("creation_date")]
    public DateTime CreationDate { get; set; }

    public UserProfile? Profile { get; set; }
    public ICollection<UserCondition> Conditions { get; set; } = [];
    public ICollection<UserMedication> Medications { get; set; } = [];
    public ICollection<BodyMetrics> BodyMetrics { get; set; } = [];
    public ICollection<HealthRecord> HealthRecords { get; set; } = [];
    public ICollection<PersonalRange> PersonalRanges { get; set; } = [];
    public ICollection<Alert> Alerts { get; set; } = [];
}