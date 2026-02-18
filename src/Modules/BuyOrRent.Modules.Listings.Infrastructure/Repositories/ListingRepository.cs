using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Domain.Entities;
using BuyOrRent.Modules.Listings.Domain.Enums;
using BuyOrRent.Modules.Listings.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuyOrRent.Modules.Listings.Infrastructure.Repositories;

public class ListingRepository(ListingsDbContext dbContext) : IListingRepository
{
    public async Task<Listing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Listings
            .Include(l => l.City)
                .ThenInclude(c => c.Municipality)
                    .ThenInclude(m => m.Region)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Listings
            .Include(l => l.City)
                .ThenInclude(c => c.Municipality)
                    .ThenInclude(m => m.Region)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Listing>> SearchAsync(Guid? regionId, Guid? municipalityId, Guid? cityId, PropertyType? propertyType, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Listings
            .Include(l => l.City)
                .ThenInclude(c => c.Municipality)
                    .ThenInclude(m => m.Region)
            .Where(l => l.Status == ListingStatus.Active)
            .AsQueryable();

        if (cityId.HasValue)
            query = query.Where(l => l.CityId == cityId.Value);
        else if (municipalityId.HasValue)
            query = query.Where(l => l.City.MunicipalityId == municipalityId.Value);
        else if (regionId.HasValue)
            query = query.Where(l => l.City.Municipality.RegionId == regionId.Value);

        if (propertyType.HasValue)
            query = query.Where(l => l.PropertyType == propertyType.Value);

        return await query.OrderByDescending(l => l.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Listing listing, CancellationToken cancellationToken = default)
    {
        dbContext.Listings.Add(listing);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Listing listing, CancellationToken cancellationToken = default)
    {
        dbContext.Listings.Update(listing);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var listing = await dbContext.Listings.FindAsync([id], cancellationToken);
        if (listing is not null)
        {
            dbContext.Listings.Remove(listing);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
