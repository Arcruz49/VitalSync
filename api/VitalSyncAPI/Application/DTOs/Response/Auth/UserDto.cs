namespace VitalSyncAPI.Application.DTOs.Responses;

public record UserDto(
    string name,
    string email,
    string token
);