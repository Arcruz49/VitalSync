using System.ComponentModel.DataAnnotations.Schema;

namespace VitalSyncAPI.Domain.Entities;

[Table("metric_types")]
public class MetricType
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;
    
    [Column("unit")]
    public string Unit { get; set; } = string.Empty;
    
    [Column("min_normal")]
    public double? MinNormal { get; set; }
    
    [Column("max_normal")]
    public double? MaxNormal { get; set; }
    
    [Column("icon")]
    public string Icon { get; set; } = string.Empty;
    
    [Column("sort_order")]
    public int SortOrder { get; set; }

    public ICollection<HealthRecord> Records { get; set; } = [];
}