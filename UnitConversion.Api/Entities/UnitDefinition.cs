namespace UnitConversion.Api.Entities;

/// <summary>
/// Represents a unit of measurement and its conversion formula relative to its category's base unit.
/// Formula to base unit:   baseValue = ((value + PreOffset) * Factor / Divisor) + PostOffset
/// Formula from base unit: value = (((baseValue - PostOffset) * Divisor) / Factor) - PreOffset
/// </summary>
public class UnitDefinition
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Symbol { get; init; }
    public required string CategoryCode { get; init; }

    /// <summary>
    /// Conversion factor numerator relative to the category base unit.
    /// </summary>
    public required decimal Factor { get; init; }

    /// <summary>
    /// Conversion factor denominator (defaults to 1). Useful for exact fractions like 5/9 in Fahrenheit.
    /// </summary>
    public decimal Divisor { get; init; } = 1m;

    /// <summary>
    /// Offset added before multiplying by the factor (e.g., -32 for Fahrenheit).
    /// </summary>
    public decimal PreOffset { get; init; } = 0m;

    /// <summary>
    /// Offset added after multiplying by the factor (e.g., +273.15 for Celsius/Fahrenheit to Kelvin).
    /// </summary>
    public decimal PostOffset { get; init; } = 0m;

    /// <summary>
    /// Alternative names, abbreviations, or symbols accepted for this unit (e.g., "m", "meters", "metre").
    /// </summary>
    public IReadOnlyList<string> Aliases { get; init; } = [];

    public decimal ToBaseUnit(decimal value)
    {
        return (((value + PreOffset) * Factor) / Divisor) + PostOffset;
    }

    public decimal FromBaseUnit(decimal baseValue)
    {
        return (((baseValue - PostOffset) * Divisor) / Factor) - PreOffset;
    }
}
