using Listings.Listing.Dtos;
using Listings.Listing.Features.GetMyListings;
using BuyOrRent.Web.Extensions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuyOrRent.Web.Pages.Listings;

[Authorize]
public class MyListingsModel(IQueryMediator queryMediator) : PageModel
{
    public IReadOnlyList<ListingDto> Listings { get; set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Listings = await queryMediator.QueryAsync(
            new GetMyListingsQuery { SellerId = User.GetUserId() }, cancellationToken);
    }
}
