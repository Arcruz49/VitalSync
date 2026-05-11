using Microsoft.EntityFrameworkCore;
using VitalSyncAPI.Domain.Entities;

namespace VitalSyncAPI.Infrastructure.Data;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<MetricType> MetricTypes { get; set; }
    public DbSet<HealthCondition> HealthConditions { get; set; }
    public DbSet<UserCondition> UserConditions { get; set; }
    public DbSet<HealthRecord> HealthRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserCondition>()
            .HasKey(x => new { x.UserId, x.ConditionId });

        modelBuilder.Entity<MetricType>().HasData(
            new MetricType { Id = 1, Name = "Pressão Sistólica",   Unit = "mmHg",  MinNormal = 90,   MaxNormal = 120,  Icon = "blood-pressure", SortOrder = 1 },
            new MetricType { Id = 2, Name = "Pressão Diastólica",  Unit = "mmHg",  MinNormal = 60,   MaxNormal = 80,   Icon = "blood-pressure", SortOrder = 2 },
            new MetricType { Id = 3, Name = "Glicemia",            Unit = "mg/dL", MinNormal = 70,   MaxNormal = 99,   Icon = "droplet",        SortOrder = 3 },
            new MetricType { Id = 4, Name = "Frequência Cardíaca", Unit = "bpm",   MinNormal = 60,   MaxNormal = 100,  Icon = "heart-pulse",    SortOrder = 4 },
            new MetricType { Id = 5, Name = "Peso",                Unit = "kg",    MinNormal = null, MaxNormal = null, Icon = "scale",          SortOrder = 5 },
            new MetricType { Id = 6, Name = "SpO2",                Unit = "%",     MinNormal = 95,   MaxNormal = 100,  Icon = "lungs",          SortOrder = 6 },
            new MetricType { Id = 7, Name = "Sono",                Unit = "h",     MinNormal = 7,    MaxNormal = 9,    Icon = "moon",           SortOrder = 7 },
            new MetricType { Id = 8, Name = "Humor",               Unit = "1-5",   MinNormal = null, MaxNormal = null, Icon = "smile",          SortOrder = 8 }
        );

        modelBuilder.Entity<HealthCondition>().HasData(
            new HealthCondition { Id = 1, Name = "Hipertensão" },
            new HealthCondition { Id = 2, Name = "Diabetes tipo 1" },
            new HealthCondition { Id = 3, Name = "Diabetes tipo 2" },
            new HealthCondition { Id = 4, Name = "Obesidade" },
            new HealthCondition { Id = 5, Name = "Doença cardíaca" },
            new HealthCondition { Id = 6, Name = "Asma" },
            new HealthCondition { Id = 7, Name = "Doença renal crônica" }
        );
    }
}