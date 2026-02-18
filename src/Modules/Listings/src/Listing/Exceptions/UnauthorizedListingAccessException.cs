namespace Listings.Listing.Exceptions;

public class UnauthorizedListingAccessException(Guid listingId)
    : Exception($"User is not authorized to modify listing {listingId}.");
