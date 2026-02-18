namespace Ratings.Rating.Dtos;

public record RatingSummaryDto(double AverageStars, int TotalRatings, int? UserRating);
