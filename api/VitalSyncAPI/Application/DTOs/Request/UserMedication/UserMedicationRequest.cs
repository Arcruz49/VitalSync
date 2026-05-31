using VitalSyncAPI.Domain.Enums;

namespace VitalSyncAPI.Application.DTOs.Request;

public record UserMedicationRequest(
    MedicationClass MedicationClass,
    string? Notes
);