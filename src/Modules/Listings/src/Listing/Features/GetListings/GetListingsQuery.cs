using Listings.Listing.Dtos;
using Listings.Listing.Enums;
using Listings.Listing.Abstractions;
using LiteBus.Queries.Abstractions;

namespace Listings.Listing.Features.GetListings;

public class GetListingsQuery : IQuery<IReadOnlyList<ListingDto>>
{
    public class Handler(IListingRepository repository) : IQueryHandler<GetListingsQuery, IReadOnlyList<ListingDto>>
    {
        public async Task<IReadOnlyList<ListingDto>> HandleAsync(GetListingsQuery query, CancellationToken cancellationToken = default)
        {
            var listings = await repository.GetAllAsync(cancellationToken);

            return listings
                .Where(l => l.Status == ListingStatus.Active)
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
                    listing.UpdatedAt)).ToList();
        }
    }
}
