using BuyOrRent.Modules.Listings.Application.Dtos;
using BuyOrRent.Modules.Listings.Application.Features.GetListing;
using BuyOrRent.Modules.Listings.Domain.Enums;
using BuyOrRent.Web.Extensions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuyOrRent.Web.Pages.Listings;

public class DetailsModel(IQueryMediator queryMediator) : PageModel
{
    public ListingDto Listing { get; set; } = default!;

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
        return Page();
    }
}
