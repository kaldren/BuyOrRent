using Comments.Comment.Abstractions;
using LiteBus.Commands.Abstractions;

namespace Comments.Comment.Features.ReplyToComment;

public class ReplyToCommentCommand : ICommand
{
    public Guid CommentId { get; init; }
    public Guid UserId { get; init; }
    public required string UserDisplayName { get; init; }
    public required string ReplyContent { get; init; }

    public class Handler(ICommentRepository commentRepository, IListingExistenceChecker listingChecker) : ICommandHandler<ReplyToCommentCommand>
    {
        public async Task HandleAsync(ReplyToCommentCommand command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command.ReplyContent))
                throw new ArgumentException("Reply content cannot be empty.");

            if (command.ReplyContent.Length > 2000)
                throw new ArgumentException("Reply content cannot exceed 2000 characters.");

            var comment = await commentRepository.GetByIdAsync(command.CommentId, cancellationToken)
                ?? throw new KeyNotFoundException("Comment not found.");

            var sellerId = await listingChecker.GetSellerIdAsync(comment.ListingId, cancellationToken)
                ?? throw new KeyNotFoundException("Listing not found.");

            if (sellerId != command.UserId)
                throw new UnauthorizedAccessException("Only the listing owner can reply to comments.");

            comment.OwnerReply = command.ReplyContent.Trim();
            comment.OwnerReplyDisplayName = command.UserDisplayName;
            comment.OwnerReplyAt = DateTime.UtcNow;

            await commentRepository.UpdateAsync(comment, cancellationToken);
        }
    }
}
