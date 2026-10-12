namespace UnitConversion.Api.Entities;

/// <summary>
/// Represents a category of measurement (e.g., Length, Temperature, Weight/Mass)
/// and defines the canonical base unit used as a pivot for conversions in this category.
/// </summary>
public class UnitCategory
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string BaseUnitCode { get; init; }
    public decimal? MinimumBaseValue { get; init; }
}
