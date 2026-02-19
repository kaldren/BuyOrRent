using Comments.Comment.Abstractions;
using Comments.Comment.Entities;
using LiteBus.Commands.Abstractions;

namespace Comments.Comment.Features.EditComment;

public class EditCommentCommand : ICommand
{
    public Guid CommentId { get; init; }
    public required string NewContent { get; init; }
    public Guid AdminUserId { get; init; }
    public required string AdminDisplayName { get; init; }

    public class Handler(ICommentRepository commentRepository) : ICommandHandler<EditCommentCommand>
    {
        public async Task HandleAsync(EditCommentCommand command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command.NewContent))
                throw new ArgumentException("Comment content cannot be empty.");

            if (command.NewContent.Length > 2000)
                throw new ArgumentException("Comment content cannot exceed 2000 characters.");

            var comment = await commentRepository.GetByIdAsync(command.CommentId, cancellationToken)
                ?? throw new KeyNotFoundException("Comment not found.");

            var auditLog = new CommentAuditLog
            {
                Id = Guid.NewGuid(),
                CommentId = comment.Id,
                ListingId = comment.ListingId,
                Action = "Edited",
                AdminUserId = command.AdminUserId,
                AdminDisplayName = command.AdminDisplayName,
                OldContent = comment.Content,
                NewContent = command.NewContent.Trim(),
                CommentAuthorName = comment.UserDisplayName,
                Timestamp = DateTime.UtcNow
            };

            comment.Content = command.NewContent.Trim();

            await commentRepository.UpdateAsync(comment, cancellationToken);
            await commentRepository.AddAuditLogAsync(auditLog, cancellationToken);
        }
    }
}
