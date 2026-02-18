using Listings.Listing.Enums;
using Listings.Listing.Entities;

namespace Listings.Listing.Abstractions;

public interface IListingRepository
{
    Task<Entities.Listing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Entities.Listing>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Entities.Listing>> SearchAsync(Guid? regionId, Guid? municipalityId, Guid? cityId, PropertyType? propertyType, CancellationToken cancellationToken = default);
    Task AddAsync(Entities.Listing listing, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.Listing listing, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
