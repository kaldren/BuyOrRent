using BuyOrRent.Modules.Listings.Application.Features.ArchiveListing;
using BuyOrRent.Modules.Listings.Application.Features.CreateListing;
using BuyOrRent.Modules.Listings.Application.Features.DeleteListing;
using BuyOrRent.Modules.Listings.Application.Features.EditListing;
using BuyOrRent.Modules.Listings.Application.Features.GetListing;
using BuyOrRent.Modules.Listings.Application.Features.GetListings;
using BuyOrRent.Modules.Listings.Application.Features.UnarchiveListing;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BuyOrRent.Modules.Listings.API.Controllers;

public static class ListingsApi
{
    public static IEndpointRouteBuilder MapListingsApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/listings").WithTags("Listings");

        group.MapGet("/", async (IQueryMediator queryMediator, CancellationToken cancellationToken) =>
        {
            var listings = await queryMediator.QueryAsync(new GetListingsQuery(), cancellationToken);
            return Results.Ok(listings);
        });

        group.MapGet("/{id:guid}", async (Guid id, IQueryMediator queryMediator, CancellationToken cancellationToken) =>
        {
            var listing = await queryMediator.QueryAsync(new GetListingQuery { Id = id }, cancellationToken);
            return listing is not null ? Results.Ok(listing) : Results.NotFound();
        });

        group.MapPost("/", async (CreateListingCommand command, ICommandMediator commandMediator, CancellationToken cancellationToken) =>
        {
            var id = await commandMediator.SendAsync(command, cancellationToken);
            return Results.Created($"/api/listings/{id}", new { id });
        });

        group.MapPut("/{id:guid}", async (Guid id, EditListingCommand command, ICommandMediator commandMediator, CancellationToken cancellationToken) =>
        {
            if (id != command.Id)
                return Results.BadRequest("Route ID does not match command ID.");

            try
            {
                await commandMediator.SendAsync(command, cancellationToken);
                return Results.NoContent();
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        });

        group.MapDelete("/{id:guid}", async (Guid id, ICommandMediator commandMediator, CancellationToken cancellationToken) =>
        {
            try
            {
                await commandMediator.SendAsync(new DeleteListingCommand { Id = id }, cancellationToken);
                return Results.NoContent();
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        });

        group.MapPost("/{id:guid}/archive", async (Guid id, ICommandMediator commandMediator, CancellationToken cancellationToken) =>
        {
            try
            {
                await commandMediator.SendAsync(new ArchiveListingCommand { Id = id }, cancellationToken);
                return Results.NoContent();
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        });

        group.MapPost("/{id:guid}/unarchive", async (Guid id, ICommandMediator commandMediator, CancellationToken cancellationToken) =>
        {
            try
            {
                await commandMediator.SendAsync(new UnarchiveListingCommand { Id = id }, cancellationToken);
                return Results.NoContent();
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        });

        return app;
    }
}
