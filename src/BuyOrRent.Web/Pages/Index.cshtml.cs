using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Application.Dtos;
using BuyOrRent.Modules.Listings.Application.Features.GetRandomListings;
using BuyOrRent.Modules.Listings.Domain.Enums;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuyOrRent.Web.Pages;

public class IndexModel(IQueryMediator queryMediator, ILocationRepository locationRepository) : PageModel
{
    public IReadOnlyList<ListingDto> FeaturedListings { get; set; } = [];
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
        FeaturedListings = await queryMediator.QueryAsync(
            new GetRandomListingsQuery { Count = 5 }, cancellationToken);

        var regions = await locationRepository.GetAllRegionsAsync(cancellationToken);
        Regions = regions.Select(r => new SelectListItem(r.Name, r.Id.ToString())).ToList();
    }
}
