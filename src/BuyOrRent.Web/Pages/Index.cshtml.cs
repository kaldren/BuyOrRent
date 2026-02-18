using BuyOrRent.Modules.Listings.Application.Dtos;
using BuyOrRent.Modules.Listings.Application.Features.GetRandomListings;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuyOrRent.Web.Pages;

public class IndexModel(IQueryMediator queryMediator) : PageModel
{
    public IReadOnlyList<ListingDto> FeaturedListings { get; set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        FeaturedListings = await queryMediator.QueryAsync(
            new GetRandomListingsQuery { Count = 5 }, cancellationToken);
    }
}
