using Microsoft.EntityFrameworkCore;
using VitalSyncAPI.Domain.Entities;

namespace VitalSyncAPI.Infrastructure.Data;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<MetricType> MetricTypes => Set<MetricType>();
    public DbSet<HealthCondition> HealthConditions => Set<HealthCondition>();
    public DbSet<UserCondition> UserConditions => Set<UserCondition>();
    public DbSet<UserMedication> UserMedications => Set<UserMedication>();
    public DbSet<HealthRecord> HealthRecords => Set<HealthRecord>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<BodyMetrics> BodyMetrics => Set<BodyMetrics>();
    public DbSet<PersonalRange> PersonalRanges => Set<PersonalRange>();
    public DbSet<AIInsight> AIInsights => Set<AIInsight>();
    public DbSet<NutritionRecord> NutritionRecords => Set<NutritionRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserCondition>()
            .HasKey(x => new { x.UserId, x.ConditionId });

        modelBuilder.Entity<PersonalRange>()
            .HasIndex(x => new { x.UserId, x.MetricTypeId })
            .IsUnique();

        modelBuilder.Entity<MetricType>().HasData(
            new MetricType { Id = 1,  Name = "Pressão Sistólica",       Unit = "mmHg",  MinNormal = 90,  MaxNormal = 120,  Icon = "blood-pressure", SortOrder = 1  },
            new MetricType { Id = 2,  Name = "Pressão Diastólica",      Unit = "mmHg",  MinNormal = 60,  MaxNormal = 80,   Icon = "blood-pressure", SortOrder = 2  },
            new MetricType { Id = 3,  Name = "Glicemia em jejum",       Unit = "mg/dL", MinNormal = 70,  MaxNormal = 99,   Icon = "droplet",        SortOrder = 3  },
            new MetricType { Id = 4,  Name = "Frequência Cardíaca",     Unit = "bpm",   MinNormal = 60,  MaxNormal = 100,  Icon = "heart-pulse",    SortOrder = 4  },
            new MetricType { Id = 5,  Name = "Peso",                    Unit = "kg",    MinNormal = null,MaxNormal = null, Icon = "scale",          SortOrder = 5  },
            new MetricType { Id = 6,  Name = "SpO2",                    Unit = "%",     MinNormal = 95,  MaxNormal = 100,  Icon = "lungs",          SortOrder = 6  },
            new MetricType { Id = 7,  Name = "Sono",                    Unit = "h",     MinNormal = 7,   MaxNormal = 9,    Icon = "moon",           SortOrder = 7  },
            new MetricType { Id = 8,  Name = "Humor",                   Unit = "1-5",   MinNormal = null,MaxNormal = null, Icon = "smile",          SortOrder = 8  },
            new MetricType { Id = 9,  Name = "Temperatura Corporal",    Unit = "°C",    MinNormal = 36,  MaxNormal = 37.5, Icon = "thermometer",    SortOrder = 9  },
            new MetricType { Id = 10, Name = "Glicemia Pós-prandial",   Unit = "mg/dL", MinNormal = null,MaxNormal = 140,  Icon = "droplet",        SortOrder = 10 },
            new MetricType { Id = 11, Name = "Passos Diários",          Unit = "passos",MinNormal = 5000,MaxNormal = null, Icon = "footsteps",      SortOrder = 11 },
            new MetricType { Id = 12, Name = "Nível de Estresse",       Unit = "1-10",  MinNormal = null,MaxNormal = 4,    Icon = "brain",          SortOrder = 12 }
        );

        modelBuilder.Entity<HealthCondition>().HasData(
            new HealthCondition { Id = 1, Name = "Hipertensão" },
            new HealthCondition { Id = 2, Name = "Diabetes tipo 1" },
            new HealthCondition { Id = 3, Name = "Diabetes tipo 2" },
            new HealthCondition { Id = 4, Name = "Pré-diabetes" },
            new HealthCondition { Id = 5, Name = "Obesidade" },
            new HealthCondition { Id = 6, Name = "Doença cardíaca" },
            new HealthCondition { Id = 7, Name = "Colesterol alto" },
            new HealthCondition { Id = 8, Name = "Hipotireoidismo" },
            new HealthCondition { Id = 9, Name = "Hipertireoidismo" },
            new HealthCondition { Id = 10, Name = "Asma" },
            new HealthCondition { Id = 11, Name = "DPOC" },
            new HealthCondition { Id = 12, Name = "Doença renal crônica" },
            new HealthCondition { Id = 13, Name = "Apneia do sono" },
            new HealthCondition { Id = 14, Name = "Ansiedade" },
            new HealthCondition { Id = 15, Name = "Depressão" },
            new HealthCondition { Id = 16, Name = "Fibromialgia" }
        );
    }
}