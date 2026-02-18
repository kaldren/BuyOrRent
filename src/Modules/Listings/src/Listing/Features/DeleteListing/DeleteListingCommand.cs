using Listings.Listing.Exceptions;
using Listings.Listing.Abstractions;
using LiteBus.Commands.Abstractions;

namespace Listings.Listing.Features.DeleteListing;

public class DeleteListingCommand : ICommand
{
    public Guid Id { get; init; }
    public Guid RequestedByUserId { get; init; }

    public class Handler(IListingRepository repository) : ICommandHandler<DeleteListingCommand>
    {
        public async Task HandleAsync(DeleteListingCommand command, CancellationToken cancellationToken = default)
        {
            var listing = await repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Listing with ID {command.Id} was not found.");

            if (listing.SellerId != command.RequestedByUserId)
                throw new UnauthorizedListingAccessException(command.Id);

            await repository.DeleteAsync(command.Id, cancellationToken);
        }
    }
}
