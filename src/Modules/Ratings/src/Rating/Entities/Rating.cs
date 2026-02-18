namespace Ratings.Rating.Entities;

public class Rating
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public Guid UserId { get; set; }
    public int Stars { get; set; }
    public DateTime CreatedAt { get; set; }
}
