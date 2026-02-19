namespace Comments.Comment.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public Guid UserId { get; set; }
    public required string UserDisplayName { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? OwnerReply { get; set; }
    public string? OwnerReplyDisplayName { get; set; }
    public DateTime? OwnerReplyAt { get; set; }
}
