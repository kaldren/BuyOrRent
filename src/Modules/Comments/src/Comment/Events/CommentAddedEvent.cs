using Comments.Comment.Abstractions;
using Comments.Comment.Entities;
using LiteBus.Events.Abstractions;

namespace Comments.Comment.Events;

public class CommentAddedEvent : IEvent
{
    public Guid ListingId { get; init; }
    public Guid SellerId { get; init; }
    public required string CommentAuthorName { get; init; }

    public class Handler(INotificationRepository notificationRepository) : IEventHandler<CommentAddedEvent>
    {
        public async Task HandleAsync(CommentAddedEvent @event, CancellationToken cancellationToken = default)
        {
            await notificationRepository.AddAsync(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = @event.SellerId,
                ListingId = @event.ListingId,
                Message = $"{@event.CommentAuthorName} commented on your listing.",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            }, cancellationToken);
        }
    }
}
