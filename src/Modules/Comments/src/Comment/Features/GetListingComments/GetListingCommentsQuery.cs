using Comments.Comment.Abstractions;
using Comments.Comment.Dtos;
using LiteBus.Queries.Abstractions;

namespace Comments.Comment.Features.GetListingComments;

public class GetListingCommentsQuery : IQuery<PaginatedResult<CommentDto>>
{
    public Guid ListingId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 5;

    public class Handler(ICommentRepository commentRepository) : IQueryHandler<GetListingCommentsQuery, PaginatedResult<CommentDto>>
    {
        public async Task<PaginatedResult<CommentDto>> HandleAsync(GetListingCommentsQuery query, CancellationToken cancellationToken = default)
        {
            var (comments, totalCount) = await commentRepository.GetByListingIdPagedAsync(
                query.ListingId, query.Page, query.PageSize, cancellationToken);

            var items = comments
                .Select(c => new CommentDto(
                    c.Id,
                    c.ListingId,
                    c.UserId,
                    c.UserDisplayName,
                    c.Content,
                    c.CreatedAt,
                    c.OwnerReply,
                    c.OwnerReplyDisplayName,
                    c.OwnerReplyAt))
                .ToList();

            return new PaginatedResult<CommentDto>(items, totalCount, query.Page, query.PageSize);
        }
    }
}
