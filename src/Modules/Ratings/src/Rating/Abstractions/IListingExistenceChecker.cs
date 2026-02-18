namespace Ratings.Rating.Abstractions;

public interface IListingExistenceChecker
{
    Task<bool> ExistsAsync(Guid listingId, CancellationToken cancellationToken = default);
}
