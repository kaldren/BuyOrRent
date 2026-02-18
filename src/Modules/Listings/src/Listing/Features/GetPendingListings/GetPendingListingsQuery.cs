using Listings.Listing.Dtos;
using Listings.Listing.Enums;
using Listings.Listing.Abstractions;
using LiteBus.Queries.Abstractions;

namespace Listings.Listing.Features.GetPendingListings;

public class GetPendingListingsQuery : IQuery<IReadOnlyList<ListingDto>>
{
    public class Handler(IListingRepository repository) : IQueryHandler<GetPendingListingsQuery, IReadOnlyList<ListingDto>>
    {
        public async Task<IReadOnlyList<ListingDto>> HandleAsync(GetPendingListingsQuery query, CancellationToken cancellationToken = default)
        {
            var listings = await repository.GetAllAsync(cancellationToken);

            return listings
                .Where(l => l.Status == ListingStatus.PendingReview)
                .OrderBy(l => l.CreatedAt)
                .Select(listing => new ListingDto(
                    listing.Id,
                    listing.Title,
                    listing.Description,
                    listing.Price,
                    listing.Address,
                    listing.CityId,
                    listing.City.Name,
                    listing.City.Municipality.Name,
                    listing.City.Municipality.Region.Name,
                    listing.PropertyType,
                    listing.NumberOfBedrooms,
                    listing.NumberOfBathrooms,
                    listing.SquareFootage,
                    listing.Status,
                    listing.RejectionReason,
                    listing.SellerId,
                    listing.ImageUrls,
                    listing.CreatedAt,
                    listing.UpdatedAt))
                .ToList();
        }
    }
}
