using System.Globalization;
using UnitConversion.Api.DTOs;
using UnitConversion.Api.Entities;
using UnitConversion.Api.Interfaces;
using UnitConversion.Api.Models;

namespace UnitConversion.Api.Services;

/// <summary>
/// Service containing the core unit conversion logic.
/// Converts values by pivoting through each category's canonical base unit.
/// </summary>
public class UnitConversionService : IUnitConversionService
{
    private readonly IUnitCatalogService _catalogService;
    private readonly ILogger<UnitConversionService> _logger;

    public UnitConversionService(
        IUnitCatalogService catalogService,
        ILogger<UnitConversionService> logger)
    {
        _catalogService = catalogService;
        _logger = logger;
    }

    public ConversionResult Convert(ConvertRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.From))
        {
            return ConversionResult.Failure("INVALID_UNIT", "The 'from' unit parameter is required.");
        }

        if (string.IsNullOrWhiteSpace(request.To))
        {
            return ConversionResult.Failure("INVALID_UNIT", "The 'to' unit parameter is required.");
        }

        if (request.Precision is < 0 or > 28)
        {
            return ConversionResult.Failure("INVALID_PRECISION", "Precision must be between 0 and 28 decimal places.");
        }

        var fromUnit = _catalogService.ResolveUnit(request.From);
        if (fromUnit is null)
        {
            return ConversionResult.Failure(
                "UNIT_NOT_FOUND",
                $"Source unit '{request.From}' is not supported. Use GET /api/unitconversion/units to see supported units.");
        }

        var toUnit = _catalogService.ResolveUnit(request.To);
        if (toUnit is null)
        {
            return ConversionResult.Failure(
                "UNIT_NOT_FOUND",
                $"Target unit '{request.To}' is not supported. Use GET /api/unitconversion/units to see supported units.");
        }

        if (!string.Equals(fromUnit.CategoryCode, toUnit.CategoryCode, StringComparison.OrdinalIgnoreCase))
        {
            return ConversionResult.Failure(
                "INCOMPATIBLE_UNITS",
                $"Cannot convert from '{fromUnit.Name}' ({fromUnit.CategoryCode}) to '{toUnit.Name}' ({toUnit.CategoryCode}). Both units must belong to the same category.");
        }

        var category = _catalogService.GetCategoryByCode(fromUnit.CategoryCode)!;

        // Step 1: Convert source value to the category's canonical base unit
        decimal baseValue = fromUnit.ToBaseUnit(request.Value);

        // Validate physical lower bound (e.g., Absolute Zero 0 K for Temperature)
        if (category.MinimumBaseValue.HasValue && baseValue < category.MinimumBaseValue.Value)
        {
            decimal minAllowed = NormalizeDecimal(fromUnit.FromBaseUnit(category.MinimumBaseValue.Value));
            return ConversionResult.Failure(
                "BELOW_ABSOLUTE_ZERO",
                $"Value {request.Value} {fromUnit.Symbol} is below absolute zero ({minAllowed} {fromUnit.Symbol}).");
        }

        // Step 2: Convert from the canonical base unit to the target unit
        decimal exactValue = string.Equals(fromUnit.Code, toUnit.Code, StringComparison.OrdinalIgnoreCase)
            ? NormalizeDecimal(request.Value)
            : NormalizeDecimal(toUnit.FromBaseUnit(baseValue));

        decimal roundedValue = NormalizeDecimal(
            decimal.Round(exactValue, request.Precision, MidpointRounding.AwayFromZero));

        decimal normalizedOriginal = NormalizeDecimal(request.Value);
        string formatted = string.Create(
            CultureInfo.InvariantCulture,
            $"{normalizedOriginal} {fromUnit.Symbol} = {roundedValue} {toUnit.Symbol}");

        _logger.LogInformation(
            "Converted {OriginalValue} {FromUnit} to {ConvertedValue} {ToUnit} ({Category})",
            normalizedOriginal,
            fromUnit.Code,
            roundedValue,
            toUnit.Code,
            category.Code);

        return ConversionResult.Success(
            originalValue: normalizedOriginal,
            convertedValue: roundedValue,
            exactValue: exactValue,
            precision: request.Precision,
            fromUnit: fromUnit,
            toUnit: toUnit,
            category: category,
            formattedResult: formatted);
    }

    public IReadOnlyList<CategoryDto> GetAllCategories()
    {
        return _catalogService.GetCategories()
            .Select(category => new CategoryDto
            {
                Code = category.Code,
                Name = category.Name,
                Description = category.Description,
                BaseUnitCode = category.BaseUnitCode,
                Units = _catalogService.GetUnits(category.Code)
                    .Select(MapToUnitDto)
                    .ToList()
            })
            .ToList();
    }

    public IReadOnlyList<UnitDto> GetUnits(string? category = null)
    {
        return _catalogService.GetUnits(category)
            .Select(MapToUnitDto)
            .ToList();
    }

    private static UnitDto MapToUnitDto(UnitDefinition unit) => new()
    {
        Code = unit.Code,
        Name = unit.Name,
        Symbol = unit.Symbol,
        Category = unit.CategoryCode,
        Aliases = unit.Aliases
    };

    /// <summary>
    /// Strips trailing fractional zeros from a decimal (e.g., 212.0000 -> 212).
    /// </summary>
    private static decimal NormalizeDecimal(decimal value)
    {
        return value / 1.000000000000000000000000000000000m;
    }
}
