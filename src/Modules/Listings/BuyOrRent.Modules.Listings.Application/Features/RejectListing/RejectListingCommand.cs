using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Domain.Enums;
using BuyOrRent.Modules.Listings.Domain.Exceptions;
using LiteBus.Commands.Abstractions;

namespace BuyOrRent.Modules.Listings.Application.Features.RejectListing;

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
                throw new ListingDomainException("Само обяви чакащи преглед могат да бъдат отхвърлени.");

            listing.Status = ListingStatus.Rejected;
            listing.RejectionReason = command.Reason;
            listing.UpdatedAt = DateTime.UtcNow;

            await repository.UpdateAsync(listing, cancellationToken);
        }
    }
}
