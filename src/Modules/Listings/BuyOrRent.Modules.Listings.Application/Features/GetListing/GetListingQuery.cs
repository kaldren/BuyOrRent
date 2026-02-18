using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Application.Dtos;
using LiteBus.Queries.Abstractions;

namespace BuyOrRent.Modules.Listings.Application.Features.GetListing;

public class GetListingQuery : IQuery<ListingDto?>
{
    public Guid Id { get; init; }

    public class Handler(IListingRepository repository) : IQueryHandler<GetListingQuery, ListingDto?>
    {
        public async Task<ListingDto?> HandleAsync(GetListingQuery query, CancellationToken cancellationToken = default)
        {
            var listing = await repository.GetByIdAsync(query.Id, cancellationToken);

            if (listing is null)
                return null;

            return new ListingDto(
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
                listing.UpdatedAt);
        }
    }
}
