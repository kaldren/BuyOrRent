using Listings.Listing.Enums;
using Listings.Listing.Exceptions;
using Listings.Listing.Abstractions;
using LiteBus.Commands.Abstractions;

namespace Listings.Listing.Features.EditListing;

public class EditListingCommand : ICommand
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public decimal? Price { get; init; }
    public required string Address { get; init; }
    public Guid CityId { get; init; }
    public PropertyType PropertyType { get; init; }
    public int NumberOfBedrooms { get; init; }
    public int NumberOfBathrooms { get; init; }
    public double SquareFootage { get; init; }
    public List<string> ImageUrls { get; init; } = [];
    public Guid RequestedByUserId { get; init; }

    public class Handler(IListingRepository repository) : ICommandHandler<EditListingCommand>
    {
        public async Task HandleAsync(EditListingCommand command, CancellationToken cancellationToken = default)
        {
            var listing = await repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Listing with ID {command.Id} was not found.");

            if (listing.SellerId != command.RequestedByUserId)
                throw new UnauthorizedListingAccessException(command.Id);

            if (listing.Status == ListingStatus.Archived)
                throw new ListingDomainException("Archived listings cannot be edited.");

            listing.Title = command.Title;
            listing.Description = command.Description;
            listing.Price = command.Price;
            listing.Address = command.Address;
            listing.CityId = command.CityId;
            listing.PropertyType = command.PropertyType;
            listing.NumberOfBedrooms = command.NumberOfBedrooms;
            listing.NumberOfBathrooms = command.NumberOfBathrooms;
            listing.SquareFootage = command.SquareFootage;
            listing.ImageUrls = command.ImageUrls;
            listing.Status = ListingStatus.PendingReview;
            listing.RejectionReason = null;
            listing.UpdatedAt = DateTime.UtcNow;

            listing.Validate();

            await repository.UpdateAsync(listing, cancellationToken);
        }
    }
}
