namespace VitalSyncAPI.Domain.Enums;

public enum AlertSeverity
{
    Warning = 1,  // valor fora do normal mas próximo do limite
    Critical = 2  // valor muito fora do normal
}