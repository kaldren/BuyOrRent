using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Application.Dtos;
using BuyOrRent.Modules.Listings.Domain.Enums;
using LiteBus.Queries.Abstractions;

namespace BuyOrRent.Modules.Listings.Application.Features.GetRandomListings;

public class GetRandomListingsQuery : IQuery<IReadOnlyList<ListingDto>>
{
    public int Count { get; init; } = 5;

    public class Handler(IListingRepository repository) : IQueryHandler<GetRandomListingsQuery, IReadOnlyList<ListingDto>>
    {
        public async Task<IReadOnlyList<ListingDto>> HandleAsync(GetRandomListingsQuery query, CancellationToken cancellationToken = default)
        {
            var all = await repository.GetAllAsync(cancellationToken);

            return all
                .Where(l => l.Status == ListingStatus.Active)
                .OrderBy(_ => Guid.NewGuid())
                .Take(query.Count)
                .Select(listing => new ListingDto(
                    listing.Id,
                    listing.Title,
                    listing.Description,
                    listing.Price,
                    listing.Address,
                    listing.City,
                    listing.State,
                    listing.ZipCode,
                    listing.NumberOfBedrooms,
                    listing.NumberOfBathrooms,
                    listing.SquareFootage,
                    listing.Status,
                    listing.SellerId,
                    listing.ImageUrls,
                    listing.CreatedAt,
                    listing.UpdatedAt))
                .ToList();
        }
    }
}
