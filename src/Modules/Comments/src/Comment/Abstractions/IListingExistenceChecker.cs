namespace Comments.Comment.Abstractions;

public interface IListingExistenceChecker
{
    Task<bool> ExistsAsync(Guid listingId, CancellationToken cancellationToken = default);
    Task<Guid?> GetSellerIdAsync(Guid listingId, CancellationToken cancellationToken = default);
}
