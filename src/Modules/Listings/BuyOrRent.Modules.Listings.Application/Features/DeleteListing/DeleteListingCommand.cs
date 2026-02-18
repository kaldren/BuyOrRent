using BuyOrRent.Modules.Listings.Application.Abstractions;
using LiteBus.Commands.Abstractions;

namespace BuyOrRent.Modules.Listings.Application.Features.DeleteListing;

public class DeleteListingCommand : ICommand
{
    public Guid Id { get; init; }

    public class Handler(IListingRepository repository) : ICommandHandler<DeleteListingCommand>
    {
        public async Task HandleAsync(DeleteListingCommand command, CancellationToken cancellationToken = default)
        {
            _ = await repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Listing with ID {command.Id} was not found.");

            await repository.DeleteAsync(command.Id, cancellationToken);
        }
    }
}
