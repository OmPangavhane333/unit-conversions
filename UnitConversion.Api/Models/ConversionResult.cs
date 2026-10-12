using UnitConversion.Api.Entities;

namespace UnitConversion.Api.Models;

/// <summary>
/// Represents the internal result of a unit conversion operation.
/// </summary>
public class ConversionResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string? ErrorCode { get; init; }

    public decimal OriginalValue { get; init; }
    public decimal ConvertedValue { get; init; }
    public decimal ExactValue { get; init; }
    public int Precision { get; init; }
    public UnitDefinition? FromUnit { get; init; }
    public UnitDefinition? ToUnit { get; init; }
    public UnitCategory? Category { get; init; }
    public string FormattedResult { get; init; } = string.Empty;

    public static ConversionResult Failure(string errorCode, string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorCode = errorCode,
        ErrorMessage = errorMessage
    };

    public static ConversionResult Success(
        decimal originalValue,
        decimal convertedValue,
        decimal exactValue,
        int precision,
        UnitDefinition fromUnit,
        UnitDefinition toUnit,
        UnitCategory category,
        string formattedResult) => new()
    {
        IsSuccess = true,
        OriginalValue = originalValue,
        ConvertedValue = convertedValue,
        ExactValue = exactValue,
        Precision = precision,
        FromUnit = fromUnit,
        ToUnit = toUnit,
        Category = category,
        FormattedResult = formattedResult
    };
}
