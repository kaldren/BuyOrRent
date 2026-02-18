using Ratings.Rating.Abstractions;
using LiteBus.Queries.Abstractions;

namespace Ratings.Rating.Features.GetListingRatingSummaries;

public class GetListingRatingSummariesQuery : IQuery<Dictionary<Guid, (double Average, int Count)>>
{
    public required IEnumerable<Guid> ListingIds { get; init; }

    public class Handler(IRatingRepository ratingRepository) : IQueryHandler<GetListingRatingSummariesQuery, Dictionary<Guid, (double Average, int Count)>>
    {
        public async Task<Dictionary<Guid, (double Average, int Count)>> HandleAsync(GetListingRatingSummariesQuery query, CancellationToken cancellationToken = default)
        {
            return await ratingRepository.GetAveragesByListingIdsAsync(query.ListingIds, cancellationToken);
        }
    }
}
