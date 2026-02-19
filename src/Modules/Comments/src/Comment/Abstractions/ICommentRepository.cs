using Comments.Comment.Entities;

namespace Comments.Comment.Abstractions;

public interface ICommentRepository
{
    Task<Entities.Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Entities.Comment?> GetByListingAndUserAsync(Guid listingId, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Entities.Comment>> GetByListingIdAsync(Guid listingId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Entities.Comment> Items, int TotalCount)> GetByListingIdPagedAsync(Guid listingId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(Entities.Comment comment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.Comment comment, CancellationToken cancellationToken = default);
    Task DeleteAsync(Entities.Comment comment, CancellationToken cancellationToken = default);
    Task AddAuditLogAsync(CommentAuditLog log, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommentAuditLog>> GetAuditLogsAsync(CancellationToken cancellationToken = default);
}
