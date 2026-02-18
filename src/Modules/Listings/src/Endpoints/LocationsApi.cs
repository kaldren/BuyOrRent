using Locations.Location.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Listings.Extensions;

public static class LocationsApi
{
    public static IEndpointRouteBuilder MapLocationsApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/locations").WithTags("Locations");

        group.MapGet("/regions", async (ILocationRepository repo, CancellationToken ct) =>
        {
            var regions = await repo.GetAllRegionsAsync(ct);
            return Results.Ok(regions.Select(r => new { r.Id, r.Name }));
        });

        group.MapGet("/municipalities/{regionId:guid}", async (Guid regionId, ILocationRepository repo, CancellationToken ct) =>
        {
            var municipalities = await repo.GetMunicipalitiesByRegionAsync(regionId, ct);
            return Results.Ok(municipalities.Select(m => new { m.Id, m.Name }));
        });

        group.MapGet("/cities/{municipalityId:guid}", async (Guid municipalityId, ILocationRepository repo, CancellationToken ct) =>
        {
            var cities = await repo.GetCitiesByMunicipalityAsync(municipalityId, ct);
            return Results.Ok(cities.Select(c => new { c.Id, c.Name }));
        });

        return app;
    }
}
