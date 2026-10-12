using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using UnitConversion.Api.DTOs;
using UnitConversion.Api.Interfaces;
using UnitConversion.Api.Models;

namespace UnitConversion.Api.Controllers;

/// <summary>
/// Single API controller exposing endpoints for converting values between units
/// and discovering supported categories and units.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UnitConversionController : ControllerBase
{
    private readonly IUnitConversionService _conversionService;
    private readonly IUnitCatalogService _catalogService;

    public UnitConversionController(
        IUnitConversionService conversionService,
        IUnitCatalogService catalogService)
    {
        _conversionService = conversionService;
        _catalogService = catalogService;
    }

    /// <summary>
    /// Converts a numerical value from one unit to another via query parameters.
    /// Example: GET /api/unitconversion/convert?value=100&amp;from=celsius&amp;to=fahrenheit
    /// </summary>
    [HttpGet("convert")]
    [ProducesResponseType(typeof(ConvertResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public ActionResult<ConvertResponseDto> ConvertViaGet(
        [FromQuery, Required] decimal? value,
        [FromQuery, Required] string? from,
        [FromQuery, Required] string? to,
        [FromQuery] int precision = 4)
    {
        var request = new ConvertRequestDto
        {
            Value = value!.Value,
            From = from!,
            To = to!,
            Precision = precision
        };

        return ExecuteConversion(request);
    }

    /// <summary>
    /// Converts a numerical value from one unit to another via a JSON request body.
    /// Example: POST /api/unitconversion/convert
    /// </summary>
    [HttpPost("convert")]
    [ProducesResponseType(typeof(ConvertResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public ActionResult<ConvertResponseDto> ConvertViaPost([FromBody] ConvertRequestDto request)
    {
        return ExecuteConversion(request);
    }

    /// <summary>
    /// Returns all supported measurement categories and their units.
    /// Example: GET /api/unitconversion/categories
    /// </summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<CategoryDto>> GetCategories()
    {
        var categories = _conversionService.GetAllCategories();
        return Ok(categories);
    }

    /// <summary>
    /// Returns all supported units, optionally filtered by category (e.g., "length", "temperature", "mass", "weight").
    /// Example: GET /api/unitconversion/units?category=length
    /// </summary>
    [HttpGet("units")]
    [ProducesResponseType(typeof(IReadOnlyList<UnitDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public ActionResult<IReadOnlyList<UnitDto>> GetUnits([FromQuery] string? category = null)
    {
        if (!string.IsNullOrWhiteSpace(category) && _catalogService.GetCategoryByCode(category) is null)
        {
            return NotFound(new ApiErrorResponse
            {
                ErrorCode = "CATEGORY_NOT_FOUND",
                Message = $"Category '{category}' was not found. Use GET /api/unitconversion/categories to view supported categories."
            });
        }

        var units = _conversionService.GetUnits(category);
        return Ok(units);
    }

    private ActionResult<ConvertResponseDto> ExecuteConversion(ConvertRequestDto request)
    {
        var result = _conversionService.Convert(request);

        if (!result.IsSuccess)
        {
            return BadRequest(new ApiErrorResponse
            {
                ErrorCode = result.ErrorCode ?? "CONVERSION_ERROR",
                Message = result.ErrorMessage ?? "Unable to perform unit conversion."
            });
        }

        var response = new ConvertResponseDto
        {
            OriginalValue = result.OriginalValue,
            FromUnit = result.FromUnit!.Code,
            FromSymbol = result.FromUnit.Symbol,
            ToUnit = result.ToUnit!.Code,
            ToSymbol = result.ToUnit.Symbol,
            Category = result.Category!.Code,
            ConvertedValue = result.ConvertedValue,
            ExactValue = result.ExactValue,
            Precision = result.Precision,
            FormattedResult = result.FormattedResult
        };

        return Ok(response);
    }
}
