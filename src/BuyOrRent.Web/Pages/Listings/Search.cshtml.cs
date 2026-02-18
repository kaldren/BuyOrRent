using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Application.Dtos;
using BuyOrRent.Modules.Listings.Application.Features.SearchListings;
using BuyOrRent.Modules.Listings.Domain.Enums;
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
    }

    private async Task LoadRegionsAsync(CancellationToken cancellationToken)
    {
        var regions = await locationRepository.GetAllRegionsAsync(cancellationToken);
        Regions = regions.Select(r => new SelectListItem(r.Name, r.Id.ToString())).ToList();
    }

    public string GetPropertyTypeDisplayName(PropertyType type) =>
        PropertyTypeHelper.GetDisplayName(type, localizer);
}
