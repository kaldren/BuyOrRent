using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Application.Dtos;
using BuyOrRent.Modules.Listings.Application.Features.GetRandomListings;
using BuyOrRent.Web.Helpers;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace BuyOrRent.Web.Pages;

public class IndexModel(IQueryMediator queryMediator, ILocationRepository locationRepository, IStringLocalizer<SharedResource> localizer) : PageModel
{
    public IReadOnlyList<ListingDto> FeaturedListings { get; set; } = [];
    public List<SelectListItem> Regions { get; set; } = [];
    public List<SelectListItem> PropertyTypeOptions { get; set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        FeaturedListings = await queryMediator.QueryAsync(
            new GetRandomListingsQuery { Count = 5 }, cancellationToken);

        var regions = await locationRepository.GetAllRegionsAsync(cancellationToken);
        Regions = regions.Select(r => new SelectListItem(r.Name, r.Id.ToString())).ToList();
        PropertyTypeOptions = PropertyTypeHelper.GetSelectListItems(localizer);
    }
}
