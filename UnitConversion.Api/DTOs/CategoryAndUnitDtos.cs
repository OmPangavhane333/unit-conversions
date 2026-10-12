namespace UnitConversion.Api.DTOs;

/// <summary>
/// DTO representing a measurement unit and its supported aliases.
/// </summary>
public class UnitDto
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Symbol { get; init; }
    public required string Category { get; init; }
    public IReadOnlyList<string> Aliases { get; init; } = [];
}

/// <summary>
/// DTO representing a measurement category and all units belonging to it.
/// </summary>
public class CategoryDto
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string BaseUnitCode { get; init; }
    public IReadOnlyList<UnitDto> Units { get; init; } = [];
}
