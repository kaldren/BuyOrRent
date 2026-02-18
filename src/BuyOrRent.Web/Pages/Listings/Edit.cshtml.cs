using System.ComponentModel.DataAnnotations;
using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Application.Features.EditListing;
using BuyOrRent.Modules.Listings.Application.Features.GetListing;
using BuyOrRent.Modules.Listings.Domain.Enums;
using BuyOrRent.Modules.Listings.Domain.Exceptions;
using BuyOrRent.Web.Extensions;
using BuyOrRent.Web.Helpers;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace BuyOrRent.Web.Pages.Listings;

[Authorize]
public class EditModel(IQueryMediator queryMediator, ICommandMediator commandMediator, IImageStorageService imageStorageService, ILocationRepository locationRepository, IStringLocalizer<SharedResource> localizer) : PageModel
{
    [BindProperty]
    public ListingInput Input { get; set; } = new();

    [BindProperty]
    public List<IFormFile> Images { get; set; } = [];

    [BindProperty]
    public List<string> RemoveImageUrls { get; set; } = [];

    public IReadOnlyList<string> CurrentImageUrls { get; set; } = [];
    public List<SelectListItem> Regions { get; set; } = [];
    public List<SelectListItem> PropertyTypeOptions { get; set; } = [];

    // Pre-selected values for cascading JS
    public Guid? SelectedRegionId { get; set; }
    public Guid? SelectedMunicipalityId { get; set; }
    public Guid? SelectedCityId { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var listing = await queryMediator.QueryAsync(new GetListingQuery { Id = id }, cancellationToken);
        if (listing is null)
            return NotFound();

        if (listing.SellerId != User.GetUserId())
            return Forbid();

        // Resolve location hierarchy for pre-selection
        var city = await locationRepository.GetCityByIdAsync(listing.CityId, cancellationToken);

        Input = new ListingInput
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            Price = listing.Price,
            Address = listing.Address,
            CityId = listing.CityId,
            PropertyType = listing.PropertyType,
            NumberOfBedrooms = listing.NumberOfBedrooms,
            NumberOfBathrooms = listing.NumberOfBathrooms,
            SquareFootage = listing.SquareFootage
        };
        CurrentImageUrls = listing.ImageUrls;

        if (city is not null)
        {
            SelectedCityId = city.Id;
            SelectedMunicipalityId = city.MunicipalityId;
            SelectedRegionId = city.Municipality.RegionId;
        }

        await LoadFormDataAsync(cancellationToken);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await ReloadPageDataAsync(cancellationToken);
            return Page();
        }

        var existing = await queryMediator.QueryAsync(new GetListingQuery { Id = Input.Id }, cancellationToken);
        if (existing is null)
            return NotFound();

        var imageUrls = existing.ImageUrls.ToList();
        foreach (var urlToRemove in RemoveImageUrls)
        {
            await imageStorageService.DeleteAsync(urlToRemove, cancellationToken);
            imageUrls.Remove(urlToRemove);
        }

        foreach (var image in Images.Where(f => f.Length > 0))
        {
            using var stream = image.OpenReadStream();
            var url = await imageStorageService.UploadAsync(
                Input.Id, stream, image.FileName, image.ContentType, cancellationToken);
            imageUrls.Add(url);
        }

        var command = new EditListingCommand
        {
            Id = Input.Id,
            Title = Input.Title,
            Description = Input.Description,
            Price = Input.Price,
            Address = Input.Address,
            CityId = Input.CityId,
            PropertyType = Input.PropertyType,
            NumberOfBedrooms = Input.NumberOfBedrooms,
            NumberOfBathrooms = Input.NumberOfBathrooms,
            SquareFootage = Input.SquareFootage,
            ImageUrls = imageUrls,
            RequestedByUserId = User.GetUserId()
        };

        try
        {
            await commandMediator.SendAsync(command, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedListingAccessException)
        {
            return Forbid();
        }
        catch (ListingDomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await ReloadPageDataAsync(cancellationToken);
            return Page();
        }

        return RedirectToPage("Index");
    }

    private async Task LoadFormDataAsync(CancellationToken cancellationToken)
    {
        var regions = await locationRepository.GetAllRegionsAsync(cancellationToken);
        Regions = regions.Select(r => new SelectListItem(r.Name, r.Id.ToString())).ToList();
        PropertyTypeOptions = PropertyTypeHelper.GetSelectListItems(localizer);
    }

    private async Task ReloadPageDataAsync(CancellationToken cancellationToken)
    {
        await LoadFormDataAsync(cancellationToken);

        var listing = await queryMediator.QueryAsync(new GetListingQuery { Id = Input.Id }, cancellationToken);
        if (listing is not null)
        {
            CurrentImageUrls = listing.ImageUrls;

            var city = await locationRepository.GetCityByIdAsync(listing.CityId, cancellationToken);
            if (city is not null)
            {
                SelectedCityId = city.Id;
                SelectedMunicipalityId = city.MunicipalityId;
                SelectedRegionId = city.Municipality.RegionId;
            }
        }
    }

    public class ListingInput
    {
        [Required]
        public Guid Id { get; set; }

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
