using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Domain.Entities;
using BuyOrRent.Modules.Listings.Domain.Enums;
using LiteBus.Commands.Abstractions;

namespace BuyOrRent.Modules.Listings.Application.Features.CreateListing;

public class CreateListingCommand : ICommand<Guid>
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public decimal? Price { get; init; }
    public required string Address { get; init; }
    public Guid CityId { get; init; }
    public required string ZipCode { get; init; }
    public int NumberOfBedrooms { get; init; }
    public int NumberOfBathrooms { get; init; }
    public double SquareFootage { get; init; }
    public Guid SellerId { get; init; }
    public List<string> ImageUrls { get; init; } = [];

    public class Handler(IListingRepository repository) : ICommandHandler<CreateListingCommand, Guid>
    {
        public async Task<Guid> HandleAsync(CreateListingCommand command, CancellationToken cancellationToken = default)
        {
            var listing = new Listing
            {
                Id = Guid.NewGuid(),
                Title = command.Title,
                Description = command.Description,
                Price = command.Price,
                Address = command.Address,
                CityId = command.CityId,
                ZipCode = command.ZipCode,
                NumberOfBedrooms = command.NumberOfBedrooms,
                NumberOfBathrooms = command.NumberOfBathrooms,
                SquareFootage = command.SquareFootage,
                Status = ListingStatus.Active,
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
