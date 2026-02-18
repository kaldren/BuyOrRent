using Ratings.Rating.Abstractions;
using Ratings.Rating.Entities;
using LiteBus.Commands.Abstractions;

namespace Ratings.Rating.Features.RateListing;

public class RateListingCommand : ICommand
{
    public Guid ListingId { get; init; }
    public Guid UserId { get; init; }
    public int Stars { get; init; }

    public class Handler(IRatingRepository ratingRepository, IListingExistenceChecker listingChecker) : ICommandHandler<RateListingCommand>
    {
        public async Task HandleAsync(RateListingCommand command, CancellationToken cancellationToken = default)
        {
            if (command.Stars is < 1 or > 5)
                throw new ArgumentException("Rating must be between 1 and 5 stars.");

            if (!await listingChecker.ExistsAsync(command.ListingId, cancellationToken))
                throw new KeyNotFoundException("Listing not found.");

            var existing = await ratingRepository.GetByListingAndUserAsync(command.ListingId, command.UserId, cancellationToken);

            if (existing is not null)
            {
                existing.Stars = command.Stars;
                await ratingRepository.UpdateAsync(existing, cancellationToken);
            }
            else
            {
                var rating = new Entities.Rating
                {
                    Id = Guid.NewGuid(),
                    ListingId = command.ListingId,
                    UserId = command.UserId,
                    Stars = command.Stars,
                    CreatedAt = DateTime.UtcNow
                };

                await ratingRepository.AddAsync(rating, cancellationToken);
            }
        }
    }
}
