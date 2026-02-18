using Listings.Listing.Dtos;
using Locations.Location.Abstractions;
using Ratings.Rating.Features.GetListingRatingSummaries;
using Listings.Listing.Features.GetRandomListings;
using BuyOrRent.Web.Helpers;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace BuyOrRent.Web.Pages;

public class IndexModel(IQueryMediator queryMediator, ILocationRepository locationRepository, IStringLocalizer<SharedResource> localizer) : PageModel
{
    public IReadOnlyList<ListingDto> FeaturedListings { get; set; } = [];
    public Dictionary<Guid, (double Average, int Count)> Ratings { get; set; } = [];
    public List<SelectListItem> Regions { get; set; } = [];
    public List<SelectListItem> PropertyTypeOptions { get; set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        FeaturedListings = await queryMediator.QueryAsync(
            new GetRandomListingsQuery { Count = 5 }, cancellationToken);

        if (FeaturedListings.Count > 0)
        {
            Ratings = await queryMediator.QueryAsync(
                new GetListingRatingSummariesQuery { ListingIds = FeaturedListings.Select(l => l.Id) },
                cancellationToken);
        }

        var regions = await locationRepository.GetAllRegionsAsync(cancellationToken);
        Regions = regions.Select(r => new SelectListItem(r.Name, r.Id.ToString())).ToList();
        PropertyTypeOptions = PropertyTypeHelper.GetSelectListItems(localizer);
    }
}
