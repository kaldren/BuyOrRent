using Listings.Data;
using Microsoft.EntityFrameworkCore;

namespace Listings.Data.Repositories;

public class ListingExistenceChecker(ListingsDbContext dbContext)
    : Ratings.Rating.Abstractions.IListingExistenceChecker,
      Comments.Comment.Abstractions.IListingExistenceChecker
{
    public async Task<bool> ExistsAsync(Guid listingId, CancellationToken cancellationToken = default)
        => await dbContext.Listings.AnyAsync(l => l.Id == listingId, cancellationToken);

    public async Task<Guid?> GetSellerIdAsync(Guid listingId, CancellationToken cancellationToken = default)
        => await dbContext.Listings
            .Where(l => l.Id == listingId)
            .Select(l => (Guid?)l.SellerId)
            .FirstOrDefaultAsync(cancellationToken);
}
