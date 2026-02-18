using Listings.Listing.Dtos;
using Listings.Listing.Enums;
using Listings.Listing.Exceptions;
using Listings.Listing.Features.GetListing;
using Ratings.Rating.Features.GetListingRatingSummary;
using Ratings.Rating.Features.RateListing;
using Ratings.Rating.Dtos;
using BuyOrRent.Web.Extensions;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuyOrRent.Web.Pages.Listings;

public class DetailsModel(IQueryMediator queryMediator, ICommandMediator commandMediator) : PageModel
{
    public ListingDto Listing { get; set; } = default!;
    public RatingSummaryDto RatingSummary { get; set; } = new(0, 0, null);

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var listing = await queryMediator.QueryAsync(new GetListingQuery { Id = id }, cancellationToken);
        if (listing is null)
            return NotFound();

        // Non-active listings are only visible to the owner or admins
        if (listing.Status != ListingStatus.Active)
        {
            var userId = User.GetUserId();
            if (listing.SellerId != userId && !User.IsAdmin())
                return NotFound();
        }

        Listing = listing;

        RatingSummary = await queryMediator.QueryAsync(new GetListingRatingSummaryQuery
        {
            ListingId = id,
            CurrentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : null
        }, cancellationToken);

        return Page();
    }

    public async Task<IActionResult> OnPostRateAsync(Guid id, int stars, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true)
            return Challenge();

        try
        {
            await commandMediator.SendAsync(new RateListingCommand
            {
                ListingId = id,
                UserId = User.GetUserId(),
                Stars = stars
            }, cancellationToken);
        }
        catch (ListingDomainException)
        {
            // Silently ignore domain errors (e.g. self-rating)
        }
        catch (ArgumentException)
        {
            // Silently ignore validation errors (e.g. invalid rating)
        }

        return RedirectToPage(new { id });
    }
}
