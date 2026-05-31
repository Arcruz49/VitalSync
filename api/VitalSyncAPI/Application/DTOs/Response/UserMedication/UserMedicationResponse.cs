namespace VitalSyncAPI.Application.DTOs.Responses;

public record UserMedicationResponse(
    Guid Id,
    string MedicationClass,
    string? Notes
);