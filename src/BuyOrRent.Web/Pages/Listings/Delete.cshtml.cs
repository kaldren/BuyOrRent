using BuyOrRent.Modules.Listings.Application.Dtos;
using BuyOrRent.Modules.Listings.Application.Features.DeleteListing;
using BuyOrRent.Modules.Listings.Application.Features.GetListing;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuyOrRent.Web.Pages.Listings;

public class DeleteModel(IQueryMediator queryMediator, ICommandMediator commandMediator) : PageModel
{
    public ListingDto Listing { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var listing = await queryMediator.QueryAsync(new GetListingQuery { Id = id }, cancellationToken);
        if (listing is null)
            return NotFound();

        Listing = listing;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await commandMediator.SendAsync(new DeleteListingCommand { Id = id }, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToPage("Index");
    }
}
