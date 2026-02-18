using Ratings.Rating.Abstractions;
using Ratings.Rating.Entities;
using Microsoft.EntityFrameworkCore;

namespace Listings.Data.Repositories;

public class RatingRepository(ListingsDbContext dbContext) : IRatingRepository
{
    public async Task<Rating?> GetByListingAndUserAsync(Guid listingId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Ratings
            .FirstOrDefaultAsync(r => r.ListingId == listingId && r.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Rating>> GetByListingIdAsync(Guid listingId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Ratings
            .Where(r => r.ListingId == listingId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, (double Average, int Count)>> GetAveragesByListingIdsAsync(IEnumerable<Guid> listingIds, CancellationToken cancellationToken = default)
    {
        var ids = listingIds.ToList();
        return await dbContext.Ratings
            .Where(r => ids.Contains(r.ListingId))
            .GroupBy(r => r.ListingId)
            .ToDictionaryAsync(
                g => g.Key,
                g => (Average: g.Average(r => (double)r.Stars), Count: g.Count()),
                cancellationToken);
    }

    public async Task AddAsync(Rating rating, CancellationToken cancellationToken = default)
    {
        dbContext.Ratings.Add(rating);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Rating rating, CancellationToken cancellationToken = default)
    {
        dbContext.Ratings.Update(rating);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
