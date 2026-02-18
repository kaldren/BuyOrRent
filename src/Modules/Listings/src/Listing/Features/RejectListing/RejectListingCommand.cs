using Listings.Listing.Enums;
using Listings.Listing.Exceptions;
using Listings.Listing.Abstractions;
using LiteBus.Commands.Abstractions;

namespace Listings.Listing.Features.RejectListing;

public class RejectListingCommand : ICommand
{
    public Guid Id { get; init; }
    public required string Reason { get; init; }

    public class Handler(IListingRepository repository) : ICommandHandler<RejectListingCommand>
    {
        public async Task HandleAsync(RejectListingCommand command, CancellationToken cancellationToken = default)
        {
            var listing = await repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Listing with ID {command.Id} was not found.");

            if (listing.Status != ListingStatus.PendingReview)
                throw new ListingDomainException("Only listings pending review can be rejected.");

            listing.Status = ListingStatus.Rejected;
            listing.RejectionReason = command.Reason;
            listing.UpdatedAt = DateTime.UtcNow;

            await repository.UpdateAsync(listing, cancellationToken);
        }
    }
}
