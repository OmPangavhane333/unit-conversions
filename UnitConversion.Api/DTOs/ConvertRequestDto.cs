using System.ComponentModel.DataAnnotations;

namespace UnitConversion.Api.DTOs;

/// <summary>
/// Request DTO for converting a value from one unit to another.
/// </summary>
public class ConvertRequestDto
{
    /// <summary>
    /// The numerical value to convert (e.g., 100).
    /// </summary>
    [Required]
    public decimal Value { get; set; }

    /// <summary>
    /// Source unit code, name, or symbol (e.g., "meters", "m", "celsius", "kg").
    /// </summary>
    [Required]
    public string From { get; set; } = string.Empty;

    /// <summary>
    /// Target unit code, name, or symbol (e.g., "feet", "ft", "fahrenheit", "lb").
    /// </summary>
    [Required]
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// Optional decimal places to round the converted value (0 to 28, default is 4).
    /// </summary>
    [Range(0, 28)]
    public int Precision { get; set; } = 4;
}
