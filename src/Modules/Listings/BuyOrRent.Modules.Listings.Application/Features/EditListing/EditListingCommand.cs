using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Domain.Enums;
using BuyOrRent.Modules.Listings.Domain.Exceptions;
using LiteBus.Commands.Abstractions;

namespace BuyOrRent.Modules.Listings.Application.Features.EditListing;

public class EditListingCommand : ICommand
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public decimal? Price { get; init; }
    public required string Address { get; init; }
    public Guid CityId { get; init; }
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
                throw new ListingDomainException("Архивирани обяви не могат да бъдат редактирани.");

            listing.Title = command.Title;
            listing.Description = command.Description;
            listing.Price = command.Price;
            listing.Address = command.Address;
            listing.CityId = command.CityId;
            listing.NumberOfBedrooms = command.NumberOfBedrooms;
            listing.NumberOfBathrooms = command.NumberOfBathrooms;
            listing.SquareFootage = command.SquareFootage;
            listing.ImageUrls = command.ImageUrls;
            listing.Status = Domain.Enums.ListingStatus.PendingReview;
            listing.RejectionReason = null;
            listing.UpdatedAt = DateTime.UtcNow;

            listing.Validate();

            await repository.UpdateAsync(listing, cancellationToken);
        }
    }
}
