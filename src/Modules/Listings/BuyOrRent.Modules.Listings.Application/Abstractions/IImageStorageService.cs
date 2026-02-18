namespace BuyOrRent.Modules.Listings.Application.Abstractions;

public interface IImageStorageService
{
    Task<string> UploadAsync(Guid listingId, Stream imageStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task DeleteAsync(string imageUrl, CancellationToken cancellationToken = default);
}
