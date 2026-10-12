namespace UnitConversion.Api.DTOs;

/// <summary>
/// Response DTO returned for a successful unit conversion.
/// </summary>
public class ConvertResponseDto
{
    public decimal OriginalValue { get; init; }
    public required string FromUnit { get; init; }
    public required string FromSymbol { get; init; }
    public required string ToUnit { get; init; }
    public required string ToSymbol { get; init; }
    public required string Category { get; init; }
    public decimal ConvertedValue { get; init; }
    public decimal ExactValue { get; init; }
    public int Precision { get; init; }
    public required string FormattedResult { get; init; }
}
