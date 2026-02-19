namespace Comments.Comment.Dtos;

public record CommentDto(
    Guid Id,
    Guid ListingId,
    Guid UserId,
    string UserDisplayName,
    string Content,
    DateTime CreatedAt,
    string? OwnerReply,
    string? OwnerReplyDisplayName,
    DateTime? OwnerReplyAt);
