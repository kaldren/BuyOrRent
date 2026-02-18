using BuyOrRent.Modules.Listings.Application.Dtos;
using BuyOrRent.Modules.Listings.Application.Features.GetPendingListings;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuyOrRent.Web.Pages.Admin;

[Authorize(Policy = "Admin")]
public class ReviewQueueModel(IQueryMediator queryMediator) : PageModel
{
    public IReadOnlyList<ListingDto> PendingListings { get; set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        PendingListings = await queryMediator.QueryAsync(new GetPendingListingsQuery(), cancellationToken);
    }
}
