using Listings.Listing.Enums;
using Listings.Listing.Exceptions;
using Listings.Listing.Abstractions;
using LiteBus.Commands.Abstractions;

namespace Listings.Listing.Features.ApproveListing;

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
                throw new ListingDomainException("Only listings pending review can be approved.");

            listing.Status = ListingStatus.Active;
            listing.RejectionReason = null;
            listing.UpdatedAt = DateTime.UtcNow;

            await repository.UpdateAsync(listing, cancellationToken);
        }
    }
}
