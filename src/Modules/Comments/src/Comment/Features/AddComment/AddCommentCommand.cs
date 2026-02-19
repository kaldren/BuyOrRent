using Comments.Comment.Abstractions;
using Comments.Comment.Entities;
using Comments.Comment.Events;
using LiteBus.Commands.Abstractions;
using LiteBus.Events.Abstractions;

namespace Comments.Comment.Features.AddComment;

public class AddCommentCommand : ICommand
{
    public Guid ListingId { get; init; }
    public Guid UserId { get; init; }
    public required string UserDisplayName { get; init; }
    public required string Content { get; init; }

    public class Handler(ICommentRepository commentRepository, IListingExistenceChecker listingChecker, IEventMediator eventMediator) : ICommandHandler<AddCommentCommand>
    {
        public async Task HandleAsync(AddCommentCommand command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command.Content))
                throw new ArgumentException("Comment content cannot be empty.");

            if (command.Content.Length > 2000)
                throw new ArgumentException("Comment content cannot exceed 2000 characters.");

            if (!await listingChecker.ExistsAsync(command.ListingId, cancellationToken))
                throw new KeyNotFoundException("Listing not found.");

            var existing = await commentRepository.GetByListingAndUserAsync(command.ListingId, command.UserId, cancellationToken);
            if (existing is not null)
                throw new InvalidOperationException("You have already commented on this listing.");

            var comment = new Entities.Comment
            {
                Id = Guid.NewGuid(),
                ListingId = command.ListingId,
                UserId = command.UserId,
                UserDisplayName = command.UserDisplayName,
                Content = command.Content.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await commentRepository.AddAsync(comment, cancellationToken);

            var sellerId = await listingChecker.GetSellerIdAsync(command.ListingId, cancellationToken);
            if (sellerId.HasValue && sellerId.Value != command.UserId)
            {
                await eventMediator.PublishAsync(new CommentAddedEvent
                {
                    ListingId = command.ListingId,
                    SellerId = sellerId.Value,
                    CommentAuthorName = command.UserDisplayName
                }, cancellationToken);
            }
        }
    }
}
