using Listings.Listing.Exceptions;
using Listings.Listing.Features.ApproveListing;
using Listings.Listing.Features.ArchiveListing;
using Listings.Listing.Features.CreateListing;
using Listings.Listing.Features.DeleteListing;
using Listings.Listing.Features.EditListing;
using Listings.Listing.Features.GetListing;
using Listings.Listing.Features.GetListings;
using Listings.Listing.Features.GetPendingListings;
using Listings.Listing.Features.RejectListing;
using Listings.Listing.Features.UnarchiveListing;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Listings.Extensions;

public static class ListingsApi
{
    private static Guid GetUserId(HttpContext context)
    {
        var oid = context.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")
               ?? context.User.FindFirstValue("oid");
        if (Guid.TryParse(oid, out var userId))
            return userId;
        var nameId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(nameId, out userId) ? userId : Guid.Empty;
    }

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

        group.MapPut("/{id:guid}", async (Guid id, EditListingCommand command, HttpContext httpContext, ICommandMediator commandMediator, CancellationToken cancellationToken) =>
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
            catch (UnauthorizedListingAccessException)
            {
                return Results.Forbid();
            }
        });

        group.MapDelete("/{id:guid}", async (Guid id, HttpContext httpContext, ICommandMediator commandMediator, CancellationToken cancellationToken) =>
        {
            try
            {
                await commandMediator.SendAsync(new DeleteListingCommand
                {
                    Id = id,
                    RequestedByUserId = GetUserId(httpContext)
                }, cancellationToken);
                return Results.NoContent();
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
            catch (UnauthorizedListingAccessException)
            {
                return Results.Forbid();
            }
        });

        group.MapPost("/{id:guid}/archive", async (Guid id, HttpContext httpContext, ICommandMediator commandMediator, CancellationToken cancellationToken) =>
        {
            try
            {
                await commandMediator.SendAsync(new ArchiveListingCommand
                {
                    Id = id,
                    RequestedByUserId = GetUserId(httpContext)
                }, cancellationToken);
                return Results.NoContent();
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
            catch (UnauthorizedListingAccessException)
            {
                return Results.Forbid();
            }
        });

        group.MapPost("/{id:guid}/unarchive", async (Guid id, HttpContext httpContext, ICommandMediator commandMediator, CancellationToken cancellationToken) =>
        {
            try
            {
                await commandMediator.SendAsync(new UnarchiveListingCommand
                {
                    Id = id,
                    RequestedByUserId = GetUserId(httpContext)
                }, cancellationToken);
                return Results.NoContent();
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
            catch (UnauthorizedListingAccessException)
            {
                return Results.Forbid();
            }
        });

        // Admin endpoints
        var adminGroup = app.MapGroup("/api/admin/listings").WithTags("Admin").RequireAuthorization("Admin");

        adminGroup.MapGet("/pending", async (IQueryMediator queryMediator, CancellationToken cancellationToken) =>
        {
            var listings = await queryMediator.QueryAsync(new GetPendingListingsQuery(), cancellationToken);
            return Results.Ok(listings);
        });

        adminGroup.MapPost("/{id:guid}/approve", async (Guid id, ICommandMediator commandMediator, CancellationToken cancellationToken) =>
        {
            try
            {
                await commandMediator.SendAsync(new ApproveListingCommand { Id = id }, cancellationToken);
                return Results.NoContent();
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
            catch (ListingDomainException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        adminGroup.MapPost("/{id:guid}/reject", async (Guid id, RejectListingRequest request, ICommandMediator commandMediator, CancellationToken cancellationToken) =>
        {
            try
            {
                await commandMediator.SendAsync(new RejectListingCommand
                {
                    Id = id,
                    Reason = request.Reason
                }, cancellationToken);
                return Results.NoContent();
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
            catch (ListingDomainException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        return app;
    }

    private record RejectListingRequest(string Reason);
}
