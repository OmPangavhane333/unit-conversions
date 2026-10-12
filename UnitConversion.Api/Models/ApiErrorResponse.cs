namespace UnitConversion.Api.Models;

/// <summary>
/// Standard error response model returned when a request is invalid.
/// </summary>
public class ApiErrorResponse
{
    public required string ErrorCode { get; init; }
    public required string Message { get; init; }
    public DateTime TimestampUtc { get; init; } = DateTime.UtcNow;
}
