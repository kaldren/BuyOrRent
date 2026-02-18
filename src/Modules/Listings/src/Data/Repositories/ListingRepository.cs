using Listings.Listing.Enums;
using Listings.Listing.Abstractions;
using Listings.Listing.Entities;
using Microsoft.EntityFrameworkCore;

namespace Listings.Data.Repositories;

public class ListingRepository(ListingsDbContext dbContext) : IListingRepository
{
    public async Task<Listing.Entities.Listing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Listings
            .Include(l => l.City)
                .ThenInclude(c => c.Municipality)
                    .ThenInclude(m => m.Region)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Listing.Entities.Listing>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Listings
            .Include(l => l.City)
                .ThenInclude(c => c.Municipality)
                    .ThenInclude(m => m.Region)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Listing.Entities.Listing>> SearchAsync(Guid? regionId, Guid? municipalityId, Guid? cityId, PropertyType? propertyType, CancellationToken cancellationToken = default)
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

    public async Task AddAsync(Listing.Entities.Listing listing, CancellationToken cancellationToken = default)
    {
        dbContext.Listings.Add(listing);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Listing.Entities.Listing listing, CancellationToken cancellationToken = default)
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
