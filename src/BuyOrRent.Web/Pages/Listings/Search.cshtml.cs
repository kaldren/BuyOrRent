using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Application.Dtos;
using BuyOrRent.Modules.Listings.Application.Features.SearchListings;
using BuyOrRent.Modules.Listings.Domain.Enums;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuyOrRent.Web.Pages.Listings;

public class SearchModel(IQueryMediator queryMediator, ILocationRepository locationRepository) : PageModel
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

    public static readonly List<SelectListItem> PropertyTypeOptions =
    [
        new("Студио", nameof(PropertyType.Studio)),
        new("Едностаен", nameof(PropertyType.OneBedroom)),
        new("Двустаен", nameof(PropertyType.TwoBedroom)),
        new("Тристаен", nameof(PropertyType.ThreeBedroom)),
        new("Четиристаен", nameof(PropertyType.FourBedroom)),
        new("Многостаен", nameof(PropertyType.MultiBedroom)),
        new("Мезонет", nameof(PropertyType.Maisonette)),
        new("Къща", nameof(PropertyType.House)),
        new("Етаж от къща", nameof(PropertyType.HouseFloor)),
        new("Гараж", nameof(PropertyType.Garage)),
        new("Парцел", nameof(PropertyType.Land)),
    ];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadRegionsAsync(cancellationToken);

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

    public static string GetPropertyTypeDisplayName(PropertyType type) => type switch
    {
        PropertyType.Studio => "Студио",
        PropertyType.OneBedroom => "Едностаен",
        PropertyType.TwoBedroom => "Двустаен",
        PropertyType.ThreeBedroom => "Тристаен",
        PropertyType.FourBedroom => "Четиристаен",
        PropertyType.MultiBedroom => "Многостаен",
        PropertyType.Maisonette => "Мезонет",
        PropertyType.House => "Къща",
        PropertyType.HouseFloor => "Етаж от къща",
        PropertyType.Garage => "Гараж",
        PropertyType.Land => "Парцел",
        _ => type.ToString()
    };
}
