using System.Text.Json.Serialization;

namespace TimeRegistration.Classes;

public record AdminRegistrationDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("userName")] string? UserName,
    [property: JsonPropertyName("phone")] string? Phone,
    [property: JsonPropertyName("checkIn")] DateTime? CheckIn,
    [property: JsonPropertyName("checkOut")] DateTime? CheckOut,
    [property: JsonPropertyName("isOpen")] bool IsOpen
);
