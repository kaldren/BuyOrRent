using Listings.Listing.Dtos;
using Listings.Listing.Features.GetListings;
using Ratings.Rating.Features.GetListingRatingSummaries;
using BuyOrRent.Web.Extensions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuyOrRent.Web.Pages.Listings;

public class IndexModel(IQueryMediator queryMediator) : PageModel
{
    public IReadOnlyList<ListingDto> Listings { get; set; } = [];
    public Dictionary<Guid, (double Average, int Count)> Ratings { get; set; } = [];
    public Guid CurrentUserId { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Listings = await queryMediator.QueryAsync(new GetListingsQuery(), cancellationToken);
        CurrentUserId = User.GetUserId();

        if (Listings.Count > 0)
        {
            Ratings = await queryMediator.QueryAsync(
                new GetListingRatingSummariesQuery { ListingIds = Listings.Select(l => l.Id) },
                cancellationToken);
        }
    }
}
