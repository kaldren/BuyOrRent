using BuyOrRent.Modules.Listings.API.Controllers;
using BuyOrRent.Modules.Listings.Application.Features.CreateListing;
using BuyOrRent.Modules.Listings.Infrastructure;
using LiteBus.Commands;
using LiteBus.Extensions.Microsoft.DependencyInjection;
using LiteBus.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuyOrRent.Modules.Listings.API;

public static class ListingsModule
{
    public static IServiceCollection AddListingsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddListingsInfrastructure(configuration);

        var assembly = typeof(CreateListingCommand).Assembly;

        services.AddLiteBus(config =>
        {
            config.AddCommandModule(builder => builder.RegisterFromAssembly(assembly));
            config.AddQueryModule(builder => builder.RegisterFromAssembly(assembly));
        });

        return services;
    }

    public static IEndpointRouteBuilder MapListingsModule(this IEndpointRouteBuilder app)
    {
        app.MapListingsApi();
        return app;
    }
}
