using Comments.Comment.Abstractions;
using Comments.Comment.Dtos;
using LiteBus.Queries.Abstractions;

namespace Comments.Comment.Features.GetCommentAuditLogs;

public class GetCommentAuditLogsQuery : IQuery<IReadOnlyList<CommentAuditLogDto>>
{
    public class Handler(ICommentRepository commentRepository) : IQueryHandler<GetCommentAuditLogsQuery, IReadOnlyList<CommentAuditLogDto>>
    {
        public async Task<IReadOnlyList<CommentAuditLogDto>> HandleAsync(GetCommentAuditLogsQuery query, CancellationToken cancellationToken = default)
        {
            var logs = await commentRepository.GetAuditLogsAsync(cancellationToken);

            return logs
                .OrderByDescending(l => l.Timestamp)
                .Select(l => new CommentAuditLogDto(
                    l.Id,
                    l.CommentId,
                    l.ListingId,
                    l.Action,
                    l.AdminDisplayName,
                    l.OldContent,
                    l.NewContent,
                    l.CommentAuthorName,
                    l.Timestamp))
                .ToList();
        }
    }
}
