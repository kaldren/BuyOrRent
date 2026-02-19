using Comments.Comment.Abstractions;
using Comments.Comment.Entities;
using LiteBus.Commands.Abstractions;

namespace Comments.Comment.Features.DeleteComment;

public class DeleteCommentCommand : ICommand
{
    public Guid CommentId { get; init; }
    public Guid AdminUserId { get; init; }
    public required string AdminDisplayName { get; init; }

    public class Handler(ICommentRepository commentRepository) : ICommandHandler<DeleteCommentCommand>
    {
        public async Task HandleAsync(DeleteCommentCommand command, CancellationToken cancellationToken = default)
        {
            var comment = await commentRepository.GetByIdAsync(command.CommentId, cancellationToken)
                ?? throw new KeyNotFoundException("Comment not found.");

            var auditLog = new CommentAuditLog
            {
                Id = Guid.NewGuid(),
                CommentId = comment.Id,
                ListingId = comment.ListingId,
                Action = "Deleted",
                AdminUserId = command.AdminUserId,
                AdminDisplayName = command.AdminDisplayName,
                OldContent = comment.Content,
                NewContent = null,
                CommentAuthorName = comment.UserDisplayName,
                Timestamp = DateTime.UtcNow
            };

            await commentRepository.DeleteAsync(comment, cancellationToken);
            await commentRepository.AddAuditLogAsync(auditLog, cancellationToken);
        }
    }
}
