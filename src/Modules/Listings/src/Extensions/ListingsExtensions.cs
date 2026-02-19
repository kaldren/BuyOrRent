using Azure.Storage.Blobs;
using Listings.Data;
using Listings.Data.Repositories;
using Listings.Listing.Abstractions;
using Listings.Storage;
using Locations.Location.Abstractions;
using Ratings;
using Ratings.Rating.Abstractions;
using Comments;
using Comments.Comment.Abstractions;
using LiteBus.Commands;
using LiteBus.Events;
using LiteBus.Extensions.Microsoft.DependencyInjection;
using LiteBus.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Listings.Extensions;

public static class ListingsExtensions
{
    public static IServiceCollection AddListingsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ListingsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ListingsDb")));

        services.AddScoped<IListingRepository, ListingRepository>();
        services.AddScoped<IRatingRepository, RatingRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<Ratings.Rating.Abstractions.IListingExistenceChecker, ListingExistenceChecker>();
        services.AddScoped<Comments.Comment.Abstractions.IListingExistenceChecker, ListingExistenceChecker>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<LocationSeeder>();

        var connectionString = configuration["AzureStorage:ConnectionString"];
        services.AddSingleton(_ => new BlobServiceClient(connectionString));
        services.AddSingleton<IImageStorageService, AzureBlobImageStorageService>();

        var listingsAssembly = typeof(ListingsRoot).Assembly;
        var ratingsAssembly = typeof(RatingsRoot).Assembly;
        var commentsAssembly = typeof(CommentsRoot).Assembly;

        services.AddLiteBus(config =>
        {
            config.AddCommandModule(builder =>
            {
                builder.RegisterFromAssembly(listingsAssembly);
                builder.RegisterFromAssembly(ratingsAssembly);
                builder.RegisterFromAssembly(commentsAssembly);
            });
            config.AddQueryModule(builder =>
            {
                builder.RegisterFromAssembly(listingsAssembly);
                builder.RegisterFromAssembly(ratingsAssembly);
                builder.RegisterFromAssembly(commentsAssembly);
            });
            config.AddEventModule(builder =>
            {
                builder.RegisterFromAssembly(commentsAssembly);
            });
        });

        return services;
    }

    public static IEndpointRouteBuilder MapListingsModule(this IEndpointRouteBuilder app)
    {
        app.MapListingsApi();
        app.MapLocationsApi();
        return app;
    }
}
