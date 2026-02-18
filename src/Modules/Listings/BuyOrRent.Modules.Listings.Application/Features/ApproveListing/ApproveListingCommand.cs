using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Domain.Enums;
using BuyOrRent.Modules.Listings.Domain.Exceptions;
using LiteBus.Commands.Abstractions;

namespace BuyOrRent.Modules.Listings.Application.Features.ApproveListing;

public class ApproveListingCommand : ICommand
{
    public Guid Id { get; init; }

    public class Handler(IListingRepository repository) : ICommandHandler<ApproveListingCommand>
    {
        public async Task HandleAsync(ApproveListingCommand command, CancellationToken cancellationToken = default)
        {
            var listing = await repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Listing with ID {command.Id} was not found.");

            if (listing.Status != ListingStatus.PendingReview)
                throw new ListingDomainException("Само обяви чакащи преглед могат да бъдат одобрени.");

            listing.Status = ListingStatus.Active;
            listing.RejectionReason = null;
            listing.UpdatedAt = DateTime.UtcNow;

            await repository.UpdateAsync(listing, cancellationToken);
        }
    }
}
