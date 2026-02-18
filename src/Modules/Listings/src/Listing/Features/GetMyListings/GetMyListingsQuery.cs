using Listings.Listing.Dtos;
using Listings.Listing.Abstractions;
using LiteBus.Queries.Abstractions;

namespace Listings.Listing.Features.GetMyListings;

public class GetMyListingsQuery : IQuery<IReadOnlyList<ListingDto>>
{
    public Guid SellerId { get; init; }

    public class Handler(IListingRepository repository) : IQueryHandler<GetMyListingsQuery, IReadOnlyList<ListingDto>>
    {
        public async Task<IReadOnlyList<ListingDto>> HandleAsync(GetMyListingsQuery query, CancellationToken cancellationToken = default)
        {
            var listings = await repository.GetAllAsync(cancellationToken);

            return listings
                .Where(l => l.SellerId == query.SellerId)
                .OrderByDescending(l => l.CreatedAt)
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
