using Listings.Listing.Enums;

namespace Listings.Listing.Dtos;

public record ListingDto(
    Guid Id,
    string Title,
    string Description,
    decimal? Price,
    string Address,
    Guid CityId,
    string CityName,
    string MunicipalityName,
    string RegionName,
    PropertyType PropertyType,
    int NumberOfBedrooms,
    int NumberOfBathrooms,
    double SquareFootage,
    ListingStatus Status,
    string? RejectionReason,
    Guid SellerId,
    IReadOnlyList<string> ImageUrls,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
