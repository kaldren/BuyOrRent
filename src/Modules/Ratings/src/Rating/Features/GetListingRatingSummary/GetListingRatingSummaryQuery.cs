using Ratings.Rating.Abstractions;
using Ratings.Rating.Dtos;
using LiteBus.Queries.Abstractions;

namespace Ratings.Rating.Features.GetListingRatingSummary;

public class GetListingRatingSummaryQuery : IQuery<RatingSummaryDto>
{
    public Guid ListingId { get; init; }
    public Guid? CurrentUserId { get; init; }

    public class Handler(IRatingRepository ratingRepository) : IQueryHandler<GetListingRatingSummaryQuery, RatingSummaryDto>
    {
        public async Task<RatingSummaryDto> HandleAsync(GetListingRatingSummaryQuery query, CancellationToken cancellationToken = default)
        {
            var ratings = await ratingRepository.GetByListingIdAsync(query.ListingId, cancellationToken);

            var totalRatings = ratings.Count;
            var averageStars = totalRatings > 0 ? ratings.Average(r => r.Stars) : 0;

            int? userRating = null;
            if (query.CurrentUserId.HasValue && query.CurrentUserId.Value != Guid.Empty)
            {
                userRating = ratings.FirstOrDefault(r => r.UserId == query.CurrentUserId.Value)?.Stars;
            }

            return new RatingSummaryDto(averageStars, totalRatings, userRating);
        }
    }
}
