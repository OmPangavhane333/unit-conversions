using UnitConversion.Api.Entities;

namespace UnitConversion.Api.Interfaces;

/// <summary>
/// Interface for storing and retrieving measurement categories and unit definitions.
/// Designed so the in-memory store can be easily replaced by a database repository in the future.
/// </summary>
public interface IUnitCatalogService
{
    IReadOnlyList<UnitCategory> GetCategories();
    UnitCategory? GetCategoryByCode(string categoryCode);
    IReadOnlyList<UnitDefinition> GetUnits(string? categoryCode = null);
    UnitDefinition? ResolveUnit(string unitIdentifier);
}
