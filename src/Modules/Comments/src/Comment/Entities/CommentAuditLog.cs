namespace Comments.Comment.Entities;

public class CommentAuditLog
{
    public Guid Id { get; set; }
    public Guid CommentId { get; set; }
    public Guid ListingId { get; set; }
    public required string Action { get; set; }
    public Guid AdminUserId { get; set; }
    public required string AdminDisplayName { get; set; }
    public required string OldContent { get; set; }
    public string? NewContent { get; set; }
    public required string CommentAuthorName { get; set; }
    public DateTime Timestamp { get; set; }
}
