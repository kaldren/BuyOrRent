using BuyOrRent.Modules.Listings.Domain.Enums;
using BuyOrRent.Modules.Listings.Domain.Exceptions;

namespace BuyOrRent.Modules.Listings.Domain.Entities;

public class Listing
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public decimal? Price { get; set; }
    public required string Address { get; set; }
    public Guid CityId { get; set; }
    public City City { get; set; } = null!;
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
            throw new ListingDomainException("Заглавието е задължително.");

        if (Title.Length > 200)
            throw new ListingDomainException("Заглавието не може да бъде по-дълго от 200 символа.");

        if (string.IsNullOrWhiteSpace(Description))
            throw new ListingDomainException("Описанието е задължително.");

        if (Description.Length > 4000)
            throw new ListingDomainException("Описанието не може да бъде по-дълго от 4000 символа.");

        if (Price is < 0)
            throw new ListingDomainException("Цената не може да бъде отрицателна.");

        if (string.IsNullOrWhiteSpace(Address))
            throw new ListingDomainException("Адресът е задължителен.");

        if (Address.Length > 300)
            throw new ListingDomainException("Адресът не може да бъде по-дълъг от 300 символа.");

        if (CityId == Guid.Empty)
            throw new ListingDomainException("Населеното място е задължително.");

        if (NumberOfBedrooms < 0)
            throw new ListingDomainException("Броят спални не може да бъде отрицателен.");

        if (NumberOfBathrooms < 0)
            throw new ListingDomainException("Броят бани не може да бъде отрицателен.");

        if (SquareFootage < 0)
            throw new ListingDomainException("Квадратурата не може да бъде отрицателна.");

        if (SellerId == Guid.Empty)
            throw new ListingDomainException("Продавачът е задължителен.");
    }
}
