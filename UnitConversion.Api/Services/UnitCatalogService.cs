using UnitConversion.Api.Entities;
using UnitConversion.Api.Interfaces;

namespace UnitConversion.Api.Services;

/// <summary>
/// Service that stores all measurement categories and unit definitions in memory
/// and provides fast, case-insensitive lookup by code, name, symbol, or alias.
/// </summary>
public class UnitCatalogService : IUnitCatalogService
{
    private readonly List<UnitCategory> _categories;
    private readonly List<UnitDefinition> _units;
    private readonly Dictionary<string, UnitCategory> _categoryLookup;
    private readonly Dictionary<string, UnitDefinition> _unitLookup;

    public UnitCatalogService()
    {
        _categories = SeedCategories();
        _units = SeedUnits();

        _categoryLookup = new Dictionary<string, UnitCategory>(StringComparer.OrdinalIgnoreCase);
        foreach (var category in _categories)
        {
            _categoryLookup[category.Code] = category;
            _categoryLookup[category.Name] = category;
        }

        // Support "weight" as an alias for "mass"
        if (_categoryLookup.TryGetValue("mass", out var massCategory))
        {
            _categoryLookup["weight"] = massCategory;
        }

        _unitLookup = new Dictionary<string, UnitDefinition>(StringComparer.OrdinalIgnoreCase);
        foreach (var unit in _units)
        {
            _unitLookup[unit.Code] = unit;
            _unitLookup.TryAdd(unit.Name, unit);
            _unitLookup.TryAdd(unit.Symbol, unit);

            foreach (var alias in unit.Aliases)
            {
                _unitLookup.TryAdd(alias, unit);
            }
        }
    }

    public IReadOnlyList<UnitCategory> GetCategories() => _categories;

    public UnitCategory? GetCategoryByCode(string categoryCode)
    {
        if (string.IsNullOrWhiteSpace(categoryCode))
        {
            return null;
        }

        return _categoryLookup.GetValueOrDefault(categoryCode.Trim());
    }

    public IReadOnlyList<UnitDefinition> GetUnits(string? categoryCode = null)
    {
        if (string.IsNullOrWhiteSpace(categoryCode))
        {
            return _units;
        }

        var category = GetCategoryByCode(categoryCode);
        if (category is null)
        {
            return [];
        }

        return _units
            .Where(u => string.Equals(u.CategoryCode, category.Code, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public UnitDefinition? ResolveUnit(string unitIdentifier)
    {
        if (string.IsNullOrWhiteSpace(unitIdentifier))
        {
            return null;
        }

        return _unitLookup.GetValueOrDefault(unitIdentifier.Trim());
    }

    private static List<UnitCategory> SeedCategories() =>
    [
        new UnitCategory
        {
            Code = "length",
            Name = "Length",
            Description = "Linear distance measurements based on the meter (m).",
            BaseUnitCode = "meter"
        },
        new UnitCategory
        {
            Code = "temperature",
            Name = "Temperature",
            Description = "Temperature scales based on Kelvin (K), bounded by absolute zero (0 K).",
            BaseUnitCode = "kelvin",
            MinimumBaseValue = 0m
        },
        new UnitCategory
        {
            Code = "mass",
            Name = "Weight / Mass",
            Description = "Mass and weight measurements based on the kilogram (kg).",
            BaseUnitCode = "kilogram"
        },
        new UnitCategory
        {
            Code = "volume",
            Name = "Volume",
            Description = "Liquid and capacity measurements based on the liter (L).",
            BaseUnitCode = "liter"
        },
        new UnitCategory
        {
            Code = "area",
            Name = "Area",
            Description = "Surface area measurements based on the square meter (m²).",
            BaseUnitCode = "square-meter"
        },
        new UnitCategory
        {
            Code = "speed",
            Name = "Speed",
            Description = "Velocity and speed measurements based on meters per second (m/s).",
            BaseUnitCode = "meter-per-second"
        },
        new UnitCategory
        {
            Code = "time",
            Name = "Time",
            Description = "Duration measurements based on the second (s).",
            BaseUnitCode = "second"
        }
    ];

    private static List<UnitDefinition> SeedUnits() =>
    [
        // ======================== LENGTH (Base: meter) ========================
        new UnitDefinition
        {
            Code = "meter",
            Name = "Meter",
            Symbol = "m",
            CategoryCode = "length",
            Factor = 1m,
            Aliases = ["m", "meter", "meters", "metre", "metres"]
        },
        new UnitDefinition
        {
            Code = "kilometer",
            Name = "Kilometer",
            Symbol = "km",
            CategoryCode = "length",
            Factor = 1000m,
            Aliases = ["km", "kilometer", "kilometers", "kilometre", "kilometres"]
        },
        new UnitDefinition
        {
            Code = "centimeter",
            Name = "Centimeter",
            Symbol = "cm",
            CategoryCode = "length",
            Factor = 0.01m,
            Aliases = ["cm", "centimeter", "centimeters", "centimetre", "centimetres"]
        },
        new UnitDefinition
        {
            Code = "millimeter",
            Name = "Millimeter",
            Symbol = "mm",
            CategoryCode = "length",
            Factor = 0.001m,
            Aliases = ["mm", "millimeter", "millimeters", "millimetre", "millimetres"]
        },
        new UnitDefinition
        {
            Code = "mile",
            Name = "Mile",
            Symbol = "mi",
            CategoryCode = "length",
            Factor = 1609.344m,
            Aliases = ["mi", "mile", "miles"]
        },
        new UnitDefinition
        {
            Code = "yard",
            Name = "Yard",
            Symbol = "yd",
            CategoryCode = "length",
            Factor = 0.9144m,
            Aliases = ["yd", "yds", "yard", "yards"]
        },
        new UnitDefinition
        {
            Code = "foot",
            Name = "Foot",
            Symbol = "ft",
            CategoryCode = "length",
            Factor = 0.3048m,
            Aliases = ["ft", "foot", "feet"]
        },
        new UnitDefinition
        {
            Code = "inch",
            Name = "Inch",
            Symbol = "in",
            CategoryCode = "length",
            Factor = 0.0254m,
            Aliases = ["in", "inch", "inches"]
        },
        new UnitDefinition
        {
            Code = "nautical-mile",
            Name = "Nautical Mile",
            Symbol = "nmi",
            CategoryCode = "length",
            Factor = 1852m,
            Aliases = ["nmi", "nautical-mile", "nautical mile", "nautical miles"]
        },

        // ======================== TEMPERATURE (Base: kelvin) ========================
        new UnitDefinition
        {
            Code = "kelvin",
            Name = "Kelvin",
            Symbol = "K",
            CategoryCode = "temperature",
            Factor = 1m,
            Divisor = 1m,
            PreOffset = 0m,
            PostOffset = 0m,
            Aliases = ["k", "kelvin", "kelvins"]
        },
        new UnitDefinition
        {
            Code = "celsius",
            Name = "Celsius",
            Symbol = "°C",
            CategoryCode = "temperature",
            Factor = 1m,
            Divisor = 1m,
            PreOffset = 0m,
            PostOffset = 273.15m,
            Aliases = ["c", "°c", "celsius", "celcius", "centigrade"]
        },
        new UnitDefinition
        {
            Code = "fahrenheit",
            Name = "Fahrenheit",
            Symbol = "°F",
            CategoryCode = "temperature",
            Factor = 5m,
            Divisor = 9m,
            PreOffset = -32m,
            PostOffset = 273.15m,
            Aliases = ["f", "°f", "fahrenheit"]
        },
        new UnitDefinition
        {
            Code = "rankine",
            Name = "Rankine",
            Symbol = "°R",
            CategoryCode = "temperature",
            Factor = 5m,
            Divisor = 9m,
            PreOffset = 0m,
            PostOffset = 0m,
            Aliases = ["r", "°r", "rankine"]
        },

        // ======================== MASS / WEIGHT (Base: kilogram) ========================
        new UnitDefinition
        {
            Code = "kilogram",
            Name = "Kilogram",
            Symbol = "kg",
            CategoryCode = "mass",
            Factor = 1m,
            Aliases = ["kg", "kgs", "kilogram", "kilograms", "kilo", "kilos"]
        },
        new UnitDefinition
        {
            Code = "gram",
            Name = "Gram",
            Symbol = "g",
            CategoryCode = "mass",
            Factor = 0.001m,
            Aliases = ["g", "gm", "gram", "grams"]
        },
        new UnitDefinition
        {
            Code = "milligram",
            Name = "Milligram",
            Symbol = "mg",
            CategoryCode = "mass",
            Factor = 0.000001m,
            Aliases = ["mg", "milligram", "milligrams"]
        },
        new UnitDefinition
        {
            Code = "metric-ton",
            Name = "Metric Ton",
            Symbol = "t",
            CategoryCode = "mass",
            Factor = 1000m,
            Aliases = ["t", "tonne", "tonnes", "metric-ton", "metric ton", "metric tons"]
        },
        new UnitDefinition
        {
            Code = "pound",
            Name = "Pound",
            Symbol = "lb",
            CategoryCode = "mass",
            Factor = 0.45359237m,
            Aliases = ["lb", "lbs", "pound", "pounds"]
        },
        new UnitDefinition
        {
            Code = "ounce",
            Name = "Ounce",
            Symbol = "oz",
            CategoryCode = "mass",
            Factor = 0.028349523125m,
            Aliases = ["oz", "ounce", "ounces"]
        },
        new UnitDefinition
        {
            Code = "stone",
            Name = "Stone",
            Symbol = "st",
            CategoryCode = "mass",
            Factor = 6.35029318m,
            Aliases = ["st", "stone", "stones"]
        },

        // ======================== VOLUME (Base: liter) ========================
        new UnitDefinition
        {
            Code = "liter",
            Name = "Liter",
            Symbol = "L",
            CategoryCode = "volume",
            Factor = 1m,
            Aliases = ["l", "ltr", "liter", "liters", "litre", "litres"]
        },
        new UnitDefinition
        {
            Code = "milliliter",
            Name = "Milliliter",
            Symbol = "mL",
            CategoryCode = "volume",
            Factor = 0.001m,
            Aliases = ["ml", "milliliter", "milliliters", "millilitre", "millilitres"]
        },
        new UnitDefinition
        {
            Code = "cubic-meter",
            Name = "Cubic Meter",
            Symbol = "m³",
            CategoryCode = "volume",
            Factor = 1000m,
            Aliases = ["m³", "m3", "cubic-meter", "cubic meter", "cubic meters"]
        },
        new UnitDefinition
        {
            Code = "us-gallon",
            Name = "US Gallon",
            Symbol = "gal",
            CategoryCode = "volume",
            Factor = 3.785411784m,
            Aliases = ["gal", "gallon", "gallons", "us-gallon", "us gallon"]
        },
        new UnitDefinition
        {
            Code = "us-quart",
            Name = "US Quart",
            Symbol = "qt",
            CategoryCode = "volume",
            Factor = 0.946352946m,
            Aliases = ["qt", "quart", "quarts", "us-quart"]
        },
        new UnitDefinition
        {
            Code = "us-pint",
            Name = "US Pint",
            Symbol = "pt",
            CategoryCode = "volume",
            Factor = 0.473176473m,
            Aliases = ["pt", "pint", "pints", "us-pint"]
        },
        new UnitDefinition
        {
            Code = "us-cup",
            Name = "US Cup",
            Symbol = "cup",
            CategoryCode = "volume",
            Factor = 0.2365882365m,
            Aliases = ["cup", "cups", "us-cup"]
        },
        new UnitDefinition
        {
            Code = "us-fluid-ounce",
            Name = "US Fluid Ounce",
            Symbol = "fl oz",
            CategoryCode = "volume",
            Factor = 0.0295735295625m,
            Aliases = ["fl-oz", "fl oz", "floz", "fluid-ounce", "fluid ounce", "fluid ounces"]
        },

        // ======================== AREA (Base: square-meter) ========================
        new UnitDefinition
        {
            Code = "square-meter",
            Name = "Square Meter",
            Symbol = "m²",
            CategoryCode = "area",
            Factor = 1m,
            Aliases = ["m²", "m2", "sqm", "square-meter", "square meter", "square meters"]
        },
        new UnitDefinition
        {
            Code = "square-kilometer",
            Name = "Square Kilometer",
            Symbol = "km²",
            CategoryCode = "area",
            Factor = 1000000m,
            Aliases = ["km²", "km2", "sqkm", "square-kilometer", "square kilometer"]
        },
        new UnitDefinition
        {
            Code = "square-foot",
            Name = "Square Foot",
            Symbol = "ft²",
            CategoryCode = "area",
            Factor = 0.09290304m,
            Aliases = ["ft²", "ft2", "sqft", "square-foot", "square foot", "square feet"]
        },
        new UnitDefinition
        {
            Code = "acre",
            Name = "Acre",
            Symbol = "ac",
            CategoryCode = "area",
            Factor = 4046.8564224m,
            Aliases = ["ac", "acre", "acres"]
        },
        new UnitDefinition
        {
            Code = "hectare",
            Name = "Hectare",
            Symbol = "ha",
            CategoryCode = "area",
            Factor = 10000m,
            Aliases = ["ha", "hectare", "hectares"]
        },

        // ======================== SPEED (Base: meter-per-second) ========================
        new UnitDefinition
        {
            Code = "meter-per-second",
            Name = "Meter per Second",
            Symbol = "m/s",
            CategoryCode = "speed",
            Factor = 1m,
            Aliases = ["m/s", "mps", "meter-per-second", "meters per second"]
        },
        new UnitDefinition
        {
            Code = "kilometer-per-hour",
            Name = "Kilometer per Hour",
            Symbol = "km/h",
            CategoryCode = "speed",
            Factor = 1m,
            Divisor = 3.6m,
            Aliases = ["km/h", "kph", "kmh", "kilometer-per-hour", "kilometers per hour"]
        },
        new UnitDefinition
        {
            Code = "mile-per-hour",
            Name = "Mile per Hour",
            Symbol = "mph",
            CategoryCode = "speed",
            Factor = 0.44704m,
            Aliases = ["mph", "mile-per-hour", "miles per hour"]
        },
        new UnitDefinition
        {
            Code = "knot",
            Name = "Knot",
            Symbol = "kn",
            CategoryCode = "speed",
            Factor = 1852m,
            Divisor = 3600m,
            Aliases = ["kn", "kt", "knot", "knots"]
        },

        // ======================== TIME (Base: second) ========================
        new UnitDefinition
        {
            Code = "second",
            Name = "Second",
            Symbol = "s",
            CategoryCode = "time",
            Factor = 1m,
            Aliases = ["s", "sec", "secs", "second", "seconds"]
        },
        new UnitDefinition
        {
            Code = "millisecond",
            Name = "Millisecond",
            Symbol = "ms",
            CategoryCode = "time",
            Factor = 0.001m,
            Aliases = ["ms", "millisecond", "milliseconds"]
        },
        new UnitDefinition
        {
            Code = "minute",
            Name = "Minute",
            Symbol = "min",
            CategoryCode = "time",
            Factor = 60m,
            Aliases = ["min", "mins", "minute", "minutes"]
        },
        new UnitDefinition
        {
            Code = "hour",
            Name = "Hour",
            Symbol = "h",
            CategoryCode = "time",
            Factor = 3600m,
            Aliases = ["h", "hr", "hrs", "hour", "hours"]
        },
        new UnitDefinition
        {
            Code = "day",
            Name = "Day",
            Symbol = "d",
            CategoryCode = "time",
            Factor = 86400m,
            Aliases = ["d", "day", "days"]
        }
    ];
}
