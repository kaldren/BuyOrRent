namespace Comments.Comment.Dtos;

public record CommentAuditLogDto(
    Guid Id,
    Guid CommentId,
    Guid ListingId,
    string Action,
    string AdminDisplayName,
    string OldContent,
    string? NewContent,
    string CommentAuthorName,
    DateTime Timestamp);
