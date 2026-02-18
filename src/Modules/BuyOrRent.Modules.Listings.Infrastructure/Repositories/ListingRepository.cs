using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Domain.Entities;
using BuyOrRent.Modules.Listings.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuyOrRent.Modules.Listings.Infrastructure.Repositories;

public class ListingRepository(ListingsDbContext dbContext) : IListingRepository
{
    public async Task<Listing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Listings.FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Listings.ToListAsync(cancellationToken);
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
