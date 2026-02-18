using BuyOrRent.Modules.Listings.Domain.Enums;

namespace BuyOrRent.Modules.Listings.Application.Dtos;

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
    string ZipCode,
    int NumberOfBedrooms,
    int NumberOfBathrooms,
    double SquareFootage,
    ListingStatus Status,
    Guid SellerId,
    IReadOnlyList<string> ImageUrls,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
