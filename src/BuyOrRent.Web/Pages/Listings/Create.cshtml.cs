using System.ComponentModel.DataAnnotations;
using Listings.Listing.Enums;
using Listings.Listing.Exceptions;
using Listings.Listing.Abstractions;
using Listings.Listing.Features.CreateListing;
using Locations.Location.Abstractions;
using BuyOrRent.Web.Extensions;
using BuyOrRent.Web.Helpers;
using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace BuyOrRent.Web.Pages.Listings;

[Authorize]
public class CreateModel(ICommandMediator commandMediator, IImageStorageService imageStorageService, ILocationRepository locationRepository, IStringLocalizer<SharedResource> localizer) : PageModel
{
    [BindProperty]
    public ListingInput Input { get; set; } = new();

    [BindProperty]
    [Display(Name = "Field_ListingImages")]
    public List<IFormFile> Images { get; set; } = [];

    public List<SelectListItem> Regions { get; set; } = [];
    public List<SelectListItem> PropertyTypeOptions { get; set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadFormDataAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadFormDataAsync(cancellationToken);
            return Page();
        }

        var listingId = Guid.NewGuid();
        var imageUrls = new List<string>();

        foreach (var image in Images.Where(f => f.Length > 0))
        {
            using var stream = image.OpenReadStream();
            var url = await imageStorageService.UploadAsync(
                listingId, stream, image.FileName, image.ContentType, cancellationToken);
            imageUrls.Add(url);
        }

        var command = new CreateListingCommand
        {
            Title = Input.Title,
            Description = Input.Description,
            Price = Input.Price,
            Address = Input.Address,
            CityId = Input.CityId,
            PropertyType = Input.PropertyType,
            NumberOfBedrooms = Input.NumberOfBedrooms,
            NumberOfBathrooms = Input.NumberOfBathrooms,
            SquareFootage = Input.SquareFootage,
            SellerId = User.GetUserId(),
            ImageUrls = imageUrls
        };

        try
        {
            await commandMediator.SendAsync(command, cancellationToken);
        }
        catch (ListingDomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadFormDataAsync(cancellationToken);
            return Page();
        }

        return RedirectToPage("MyListings");
    }

    private async Task LoadFormDataAsync(CancellationToken cancellationToken)
    {
        var regions = await locationRepository.GetAllRegionsAsync(cancellationToken);
        Regions = regions.Select(r => new SelectListItem(r.Name, r.Id.ToString())).ToList();
        PropertyTypeOptions = PropertyTypeHelper.GetSelectListItems(localizer);
    }

    public class ListingInput
    {
        [Required(ErrorMessage = "Validation_TitleRequired")]
        [StringLength(200, ErrorMessage = "Validation_TitleMaxLength")]
        [Display(Name = "Field_Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Validation_DescriptionRequired")]
        [StringLength(4000, ErrorMessage = "Validation_DescriptionMaxLength")]
        [Display(Name = "Field_Description")]
        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Validation_PriceRange")]
        [Display(Name = "Field_Price")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Validation_AddressRequired")]
        [StringLength(300, ErrorMessage = "Validation_AddressMaxLength")]
        [Display(Name = "Field_Address")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Field_Region")]
        public Guid? RegionId { get; set; }

        [Display(Name = "Field_Municipality")]
        public Guid? MunicipalityId { get; set; }

        [Required(ErrorMessage = "Validation_CityRequired")]
        [Display(Name = "Field_City")]
        public Guid CityId { get; set; }

        [Required(ErrorMessage = "Validation_PropertyTypeRequired")]
        [Display(Name = "Field_PropertyType")]
        public PropertyType PropertyType { get; set; }

        [Range(0, 100, ErrorMessage = "Validation_BedroomsRange")]
        [Display(Name = "Field_Bedrooms")]
        public int NumberOfBedrooms { get; set; }

        [Range(0, 100, ErrorMessage = "Validation_BathroomsRange")]
        [Display(Name = "Field_Bathrooms")]
        public int NumberOfBathrooms { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Validation_SquareFootageRange")]
        [Display(Name = "Field_SquareFootage")]
        public double SquareFootage { get; set; }
    }
}
