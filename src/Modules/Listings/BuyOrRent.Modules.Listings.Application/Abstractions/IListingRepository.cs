using BuyOrRent.Modules.Listings.Domain.Entities;
using BuyOrRent.Modules.Listings.Domain.Enums;

namespace BuyOrRent.Modules.Listings.Application.Abstractions;

public interface IListingRepository
{
    Task<Listing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Listing>> SearchAsync(Guid? regionId, Guid? municipalityId, Guid? cityId, PropertyType? propertyType, CancellationToken cancellationToken = default);
    Task AddAsync(Listing listing, CancellationToken cancellationToken = default);
    Task UpdateAsync(Listing listing, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
