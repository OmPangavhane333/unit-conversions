using UnitConversion.Api.DTOs;
using UnitConversion.Api.Models;

namespace UnitConversion.Api.Interfaces;

/// <summary>
/// Interface for the unit conversion business logic service.
/// </summary>
public interface IUnitConversionService
{
    ConversionResult Convert(ConvertRequestDto request);
    IReadOnlyList<CategoryDto> GetAllCategories();
    IReadOnlyList<UnitDto> GetUnits(string? category = null);
}
