using Listings.Listing.Enums;
using Listings.Listing.Exceptions;

namespace Listings.Listing.Entities;

public class Listing
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public decimal? Price { get; set; }
    public required string Address { get; set; }
    public Guid CityId { get; set; }
    public Locations.Location.Entities.City City { get; set; } = null!;
    public PropertyType PropertyType { get; set; }
    public int NumberOfBedrooms { get; set; }
    public int NumberOfBathrooms { get; set; }
    public double SquareFootage { get; set; }
    public ListingStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public Guid SellerId { get; set; }
    public List<string> ImageUrls { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            throw new ListingDomainException("Title is required.");

        if (Title.Length > 200)
            throw new ListingDomainException("Title cannot be longer than 200 characters.");

        if (string.IsNullOrWhiteSpace(Description))
            throw new ListingDomainException("Description is required.");

        if (Description.Length > 4000)
            throw new ListingDomainException("Description cannot be longer than 4000 characters.");

        if (Price is < 0)
            throw new ListingDomainException("Price cannot be negative.");

        if (string.IsNullOrWhiteSpace(Address))
            throw new ListingDomainException("Address is required.");

        if (Address.Length > 300)
            throw new ListingDomainException("Address cannot be longer than 300 characters.");

        if (CityId == Guid.Empty)
            throw new ListingDomainException("City is required.");

        if (NumberOfBedrooms < 0)
            throw new ListingDomainException("Number of bedrooms cannot be negative.");

        if (NumberOfBathrooms < 0)
            throw new ListingDomainException("Number of bathrooms cannot be negative.");

        if (SquareFootage < 0)
            throw new ListingDomainException("Square footage cannot be negative.");

        if (SellerId == Guid.Empty)
            throw new ListingDomainException("Seller is required.");
    }
}
