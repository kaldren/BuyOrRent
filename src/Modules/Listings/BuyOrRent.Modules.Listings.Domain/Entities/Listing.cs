using BuyOrRent.Modules.Listings.Domain.Enums;

namespace BuyOrRent.Modules.Listings.Domain.Entities;

public class Listing
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string ZipCode { get; set; }
    public int NumberOfBedrooms { get; set; }
    public int NumberOfBathrooms { get; set; }
    public double SquareFootage { get; set; }
    public ListingStatus Status { get; set; }
    public Guid SellerId { get; set; }
    public List<string> ImageUrls { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
