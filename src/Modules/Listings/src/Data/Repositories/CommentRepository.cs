using Comments.Comment.Abstractions;
using Comments.Comment.Entities;
using Microsoft.EntityFrameworkCore;

namespace Listings.Data.Repositories;

public class CommentRepository(ListingsDbContext dbContext) : ICommentRepository
{
    public async Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Comments
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Comment?> GetByListingAndUserAsync(Guid listingId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Comments
            .FirstOrDefaultAsync(c => c.ListingId == listingId && c.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Comment>> GetByListingIdAsync(Guid listingId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Comments
            .Where(c => c.ListingId == listingId)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Comment> Items, int TotalCount)> GetByListingIdPagedAsync(
        Guid listingId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Comments
            .Where(c => c.ListingId == listingId)
            .OrderByDescending(c => c.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Comment comment, CancellationToken cancellationToken = default)
    {
        dbContext.Comments.Add(comment);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Comment comment, CancellationToken cancellationToken = default)
    {
        dbContext.Comments.Update(comment);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Comment comment, CancellationToken cancellationToken = default)
    {
        dbContext.Comments.Remove(comment);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddAuditLogAsync(CommentAuditLog log, CancellationToken cancellationToken = default)
    {
        dbContext.CommentAuditLogs.Add(log);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CommentAuditLog>> GetAuditLogsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.CommentAuditLogs
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync(cancellationToken);
    }
}
