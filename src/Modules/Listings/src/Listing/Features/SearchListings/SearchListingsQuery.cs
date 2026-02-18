using Listings.Listing.Dtos;
using Listings.Listing.Enums;
using Listings.Listing.Abstractions;
using LiteBus.Queries.Abstractions;

namespace Listings.Listing.Features.SearchListings;

public class SearchListingsQuery : IQuery<IReadOnlyList<ListingDto>>
{
    public Guid? RegionId { get; init; }
    public Guid? MunicipalityId { get; init; }
    public Guid? CityId { get; init; }
    public PropertyType? PropertyType { get; init; }

    public class Handler(IListingRepository repository) : IQueryHandler<SearchListingsQuery, IReadOnlyList<ListingDto>>
    {
        public async Task<IReadOnlyList<ListingDto>> HandleAsync(SearchListingsQuery query, CancellationToken cancellationToken = default)
        {
            var listings = await repository.SearchAsync(
                query.RegionId, query.MunicipalityId, query.CityId, query.PropertyType, cancellationToken);

            return listings
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
