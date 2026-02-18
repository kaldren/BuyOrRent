using BuyOrRent.Modules.Listings.Application.Dtos;
using BuyOrRent.Modules.Listings.Application.Features.GetListings;
using BuyOrRent.Web.Extensions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuyOrRent.Web.Pages.Listings;

public class IndexModel(IQueryMediator queryMediator) : PageModel
{
    public IReadOnlyList<ListingDto> Listings { get; set; } = [];
    public Guid CurrentUserId { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Listings = await queryMediator.QueryAsync(new GetListingsQuery(), cancellationToken);
        CurrentUserId = User.GetUserId();
    }
}
