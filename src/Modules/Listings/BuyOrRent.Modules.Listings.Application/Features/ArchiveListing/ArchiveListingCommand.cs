using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Domain.Enums;
using BuyOrRent.Modules.Listings.Domain.Exceptions;
using LiteBus.Commands.Abstractions;

namespace BuyOrRent.Modules.Listings.Application.Features.ArchiveListing;

public class ArchiveListingCommand : ICommand
{
    public Guid Id { get; init; }
    public Guid RequestedByUserId { get; init; }

    public class Handler(IListingRepository repository) : ICommandHandler<ArchiveListingCommand>
    {
        public async Task HandleAsync(ArchiveListingCommand command, CancellationToken cancellationToken = default)
        {
            var listing = await repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Listing with ID {command.Id} was not found.");

            if (listing.SellerId != command.RequestedByUserId)
                throw new UnauthorizedListingAccessException(command.Id);

            listing.Status = ListingStatus.Archived;
            listing.UpdatedAt = DateTime.UtcNow;

            await repository.UpdateAsync(listing, cancellationToken);
        }
    }
}
