using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Domain.Enums;
using LiteBus.Commands.Abstractions;

namespace BuyOrRent.Modules.Listings.Application.Features.UnarchiveListing;

public class UnarchiveListingCommand : ICommand
{
    public Guid Id { get; init; }

    public class Handler(IListingRepository repository) : ICommandHandler<UnarchiveListingCommand>
    {
        public async Task HandleAsync(UnarchiveListingCommand command, CancellationToken cancellationToken = default)
        {
            var listing = await repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Listing with ID {command.Id} was not found.");

            listing.Status = ListingStatus.Active;
            listing.UpdatedAt = DateTime.UtcNow;

            await repository.UpdateAsync(listing, cancellationToken);
        }
    }
}
