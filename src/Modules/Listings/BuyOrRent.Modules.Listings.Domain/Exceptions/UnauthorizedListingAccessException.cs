namespace BuyOrRent.Modules.Listings.Domain.Exceptions;

public class UnauthorizedListingAccessException(Guid listingId)
    : Exception($"User is not authorized to modify listing {listingId}.");
