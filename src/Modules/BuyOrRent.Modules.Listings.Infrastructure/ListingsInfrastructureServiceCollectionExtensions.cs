using Azure.Storage.Blobs;
using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Infrastructure.Persistence;
using BuyOrRent.Modules.Listings.Infrastructure.Repositories;
using BuyOrRent.Modules.Listings.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuyOrRent.Modules.Listings.Infrastructure;

public static class ListingsInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddListingsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ListingsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ListingsDb")));

        services.AddScoped<IListingRepository, ListingRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<LocationSeeder>();

        var connectionString = configuration["AzureStorage:ConnectionString"];
        services.AddSingleton(_ => new BlobServiceClient(connectionString));

        services.AddSingleton<IImageStorageService, AzureBlobImageStorageService>();

        return services;
    }
}
