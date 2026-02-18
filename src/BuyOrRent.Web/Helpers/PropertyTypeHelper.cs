using Listings.Listing.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace BuyOrRent.Web.Helpers;

public static class PropertyTypeHelper
{
    private static readonly PropertyType[] AllTypes =
    [
        PropertyType.Studio,
        PropertyType.OneBedroom,
        PropertyType.TwoBedroom,
        PropertyType.ThreeBedroom,
        PropertyType.FourBedroom,
        PropertyType.MultiBedroom,
        PropertyType.Maisonette,
        PropertyType.House,
        PropertyType.HouseFloor,
        PropertyType.Garage,
        PropertyType.Land,
    ];

    public static List<SelectListItem> GetSelectListItems(IStringLocalizer<SharedResource> localizer) =>
        AllTypes.Select(t => new SelectListItem(GetDisplayName(t, localizer), t.ToString())).ToList();

    public static string GetDisplayName(PropertyType type, IStringLocalizer<SharedResource> localizer) =>
        localizer[$"PropertyType_{type}"].Value;
}
