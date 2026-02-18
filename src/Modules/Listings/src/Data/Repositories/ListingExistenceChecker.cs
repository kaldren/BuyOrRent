using Listings.Data;
using Ratings.Rating.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Listings.Data.Repositories;

public class ListingExistenceChecker(ListingsDbContext dbContext) : IListingExistenceChecker
{
    public async Task<bool> ExistsAsync(Guid listingId, CancellationToken cancellationToken = default)
        => await dbContext.Listings.AnyAsync(l => l.Id == listingId, cancellationToken);
}
