using Listings.Listing.Dtos;
using Listings.Listing.Enums;
using Listings.Listing.Features.SearchListings;
using Locations.Location.Abstractions;
using Ratings.Rating.Features.GetListingRatingSummaries;
using BuyOrRent.Web.Helpers;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace BuyOrRent.Web.Pages.Listings;

public class SearchModel(IQueryMediator queryMediator, ILocationRepository locationRepository, IStringLocalizer<SharedResource> localizer) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? RegionId { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? MunicipalityId { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? CityId { get; set; }

    [BindProperty(SupportsGet = true)]
    public PropertyType? SelectedPropertyType { get; set; }

    public IReadOnlyList<ListingDto> Results { get; set; } = [];
    public Dictionary<Guid, (double Average, int Count)> Ratings { get; set; } = [];
    public List<SelectListItem> Regions { get; set; } = [];
    public List<SelectListItem> PropertyTypeOptions { get; set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadRegionsAsync(cancellationToken);
        PropertyTypeOptions = PropertyTypeHelper.GetSelectListItems(localizer);

        var query = new SearchListingsQuery
        {
            RegionId = RegionId,
            MunicipalityId = MunicipalityId,
            CityId = CityId,
            PropertyType = SelectedPropertyType
        };

        Results = await queryMediator.QueryAsync(query, cancellationToken);

        if (Results.Count > 0)
        {
            Ratings = await queryMediator.QueryAsync(
                new GetListingRatingSummariesQuery { ListingIds = Results.Select(l => l.Id) },
                cancellationToken);
        }
    }

    private async Task LoadRegionsAsync(CancellationToken cancellationToken)
    {
        var regions = await locationRepository.GetAllRegionsAsync(cancellationToken);
        Regions = regions.Select(r => new SelectListItem(r.Name, r.Id.ToString())).ToList();
    }

    public string GetPropertyTypeDisplayName(PropertyType type) =>
        PropertyTypeHelper.GetDisplayName(type, localizer);
}
