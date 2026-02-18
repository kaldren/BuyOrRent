using BuyOrRent.Modules.Listings.Domain.Entities;

namespace BuyOrRent.Modules.Listings.Application.Abstractions;

public interface ILocationRepository
{
    Task<IReadOnlyList<Country>> GetCountriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Region>> GetRegionsByCountryAsync(Guid countryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Municipality>> GetMunicipalitiesByRegionAsync(Guid regionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<City>> GetCitiesByMunicipalityAsync(Guid municipalityId, CancellationToken cancellationToken = default);
    Task<City?> GetCityByIdAsync(Guid cityId, CancellationToken cancellationToken = default);
    Task<bool> HasLocationsAsync(CancellationToken cancellationToken = default);
    Task SeedAsync(Country country, CancellationToken cancellationToken = default);
}
