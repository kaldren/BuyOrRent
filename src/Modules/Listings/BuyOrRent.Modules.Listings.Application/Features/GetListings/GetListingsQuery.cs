using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Application.Dtos;
using BuyOrRent.Modules.Listings.Domain.Enums;
using LiteBus.Queries.Abstractions;

namespace BuyOrRent.Modules.Listings.Application.Features.GetListings;

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
