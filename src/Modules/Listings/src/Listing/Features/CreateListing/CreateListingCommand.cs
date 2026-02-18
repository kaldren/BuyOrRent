using Listings.Listing.Enums;
using Listings.Listing.Abstractions;
using Listings.Listing.Entities;
using LiteBus.Commands.Abstractions;

namespace Listings.Listing.Features.CreateListing;

public class CreateListingCommand : ICommand<Guid>
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public decimal? Price { get; init; }
    public required string Address { get; init; }
    public Guid CityId { get; init; }
    public PropertyType PropertyType { get; init; }
    public int NumberOfBedrooms { get; init; }
    public int NumberOfBathrooms { get; init; }
    public double SquareFootage { get; init; }
    public Guid SellerId { get; init; }
    public List<string> ImageUrls { get; init; } = [];

    public class Handler(IListingRepository repository) : ICommandHandler<CreateListingCommand, Guid>
    {
        public async Task<Guid> HandleAsync(CreateListingCommand command, CancellationToken cancellationToken = default)
        {
            var listing = new Entities.Listing
            {
                Id = Guid.NewGuid(),
                Title = command.Title,
                Description = command.Description,
                Price = command.Price,
                Address = command.Address,
                CityId = command.CityId,
                PropertyType = command.PropertyType,
                NumberOfBedrooms = command.NumberOfBedrooms,
                NumberOfBathrooms = command.NumberOfBathrooms,
                SquareFootage = command.SquareFootage,
                Status = ListingStatus.PendingReview,
                SellerId = command.SellerId,
                ImageUrls = command.ImageUrls,
                CreatedAt = DateTime.UtcNow
            };

            listing.Validate();

            await repository.AddAsync(listing, cancellationToken);

            return listing.Id;
        }
    }
}
