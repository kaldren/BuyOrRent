using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BuyOrRent.Modules.Listings.Application.Abstractions;

namespace BuyOrRent.Modules.Listings.Infrastructure.Storage;

public class AzureBlobImageStorageService(BlobServiceClient blobServiceClient) : IImageStorageService
{
    private const string ContainerName = "listings";

    public async Task<string> UploadAsync(Guid listingId, Stream imageStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        var extension = Path.GetExtension(fileName);
        var blobPath = $"{listingId}/photos/{Guid.NewGuid()}{extension}";
        var blobClient = containerClient.GetBlobClient(blobPath);

        await blobClient.UploadAsync(imageStream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);

        return blobClient.Uri.ToString();
    }

    public async Task DeleteAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        var uri = new Uri(imageUrl);
        // Path is /listings/{listingId}/photos/{filename} — strip the leading container name
        var segments = uri.AbsolutePath.TrimStart('/').Split('/', 2);
        if (segments.Length < 2)
            return;

        var blobPath = segments[1]; // everything after the container name
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        var blobClient = containerClient.GetBlobClient(blobPath);

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}
