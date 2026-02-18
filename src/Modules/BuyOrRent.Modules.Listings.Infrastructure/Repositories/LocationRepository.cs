using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Domain.Entities;
using BuyOrRent.Modules.Listings.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuyOrRent.Modules.Listings.Infrastructure.Repositories;

public class LocationRepository(ListingsDbContext dbContext) : ILocationRepository
{
    public async Task<IReadOnlyList<Country>> GetCountriesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Countries.OrderBy(c => c.Name).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Region>> GetRegionsByCountryAsync(Guid countryId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Regions
            .Where(r => r.CountryId == countryId)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Municipality>> GetMunicipalitiesByRegionAsync(Guid regionId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Municipalities
            .Where(m => m.RegionId == regionId)
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<City>> GetCitiesByMunicipalityAsync(Guid municipalityId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Cities
            .Where(c => c.MunicipalityId == municipalityId)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<City?> GetCityByIdAsync(Guid cityId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Cities
            .Include(c => c.Municipality)
                .ThenInclude(m => m.Region)
                    .ThenInclude(r => r.Country)
            .FirstOrDefaultAsync(c => c.Id == cityId, cancellationToken);
    }

    public async Task<bool> HasLocationsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Countries.AnyAsync(cancellationToken);
    }

    public async Task SeedAsync(Country country, CancellationToken cancellationToken = default)
    {
        dbContext.Countries.Add(country);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
