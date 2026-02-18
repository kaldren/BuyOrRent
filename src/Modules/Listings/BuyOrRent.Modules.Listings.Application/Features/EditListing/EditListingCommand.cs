using BuyOrRent.Modules.Listings.Application.Abstractions;
using LiteBus.Commands.Abstractions;

namespace BuyOrRent.Modules.Listings.Application.Features.EditListing;

public class EditListingCommand : ICommand
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public decimal Price { get; init; }
    public required string Address { get; init; }
    public required string City { get; init; }
    public required string State { get; init; }
    public required string ZipCode { get; init; }
    public int NumberOfBedrooms { get; init; }
    public int NumberOfBathrooms { get; init; }
    public double SquareFootage { get; init; }
    public List<string> ImageUrls { get; init; } = [];

    public class Handler(IListingRepository repository) : ICommandHandler<EditListingCommand>
    {
        public async Task HandleAsync(EditListingCommand command, CancellationToken cancellationToken = default)
        {
            var listing = await repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Listing with ID {command.Id} was not found.");

            listing.Title = command.Title;
            listing.Description = command.Description;
            listing.Price = command.Price;
            listing.Address = command.Address;
            listing.City = command.City;
            listing.State = command.State;
            listing.ZipCode = command.ZipCode;
            listing.NumberOfBedrooms = command.NumberOfBedrooms;
            listing.NumberOfBathrooms = command.NumberOfBathrooms;
            listing.SquareFootage = command.SquareFootage;
            listing.ImageUrls = command.ImageUrls;
            listing.UpdatedAt = DateTime.UtcNow;

            await repository.UpdateAsync(listing, cancellationToken);
        }
    }
}
