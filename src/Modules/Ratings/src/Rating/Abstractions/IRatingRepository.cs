using Ratings.Rating.Entities;

namespace Ratings.Rating.Abstractions;

public interface IRatingRepository
{
    Task<Entities.Rating?> GetByListingAndUserAsync(Guid listingId, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Entities.Rating>> GetByListingIdAsync(Guid listingId, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, (double Average, int Count)>> GetAveragesByListingIdsAsync(IEnumerable<Guid> listingIds, CancellationToken cancellationToken = default);
    Task AddAsync(Entities.Rating rating, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.Rating rating, CancellationToken cancellationToken = default);
}
