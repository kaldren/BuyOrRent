using Listings.Listing.Dtos;
using Listings.Listing.Exceptions;
using Listings.Listing.Features.ArchiveListing;
using Listings.Listing.Features.GetListing;
using BuyOrRent.Web.Extensions;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuyOrRent.Web.Pages.Listings;

[Authorize]
public class ArchiveModel(IQueryMediator queryMediator, ICommandMediator commandMediator) : PageModel
{
    public ListingDto Listing { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var listing = await queryMediator.QueryAsync(new GetListingQuery { Id = id }, cancellationToken);
        if (listing is null)
            return NotFound();

        if (listing.SellerId != User.GetUserId())
            return Forbid();

        Listing = listing;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await commandMediator.SendAsync(new ArchiveListingCommand
            {
                Id = id,
                RequestedByUserId = User.GetUserId()
            }, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedListingAccessException)
        {
            return Forbid();
        }

        return RedirectToPage("Index");
    }
}
