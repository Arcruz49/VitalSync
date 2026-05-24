namespace VitalSyncAPI.Application.DTOs.Request;

public class SearchHealthRecordRequest{
    public int? MetricTypeId {get; set;}
    public DateTime? From {get; set;}
    public DateTime? To {get; set;}
}

