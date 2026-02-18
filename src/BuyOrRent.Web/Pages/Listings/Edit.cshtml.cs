using System.ComponentModel.DataAnnotations;
using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Application.Features.EditListing;
using BuyOrRent.Modules.Listings.Application.Features.GetListing;
using BuyOrRent.Modules.Listings.Domain.Exceptions;
using BuyOrRent.Web.Extensions;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuyOrRent.Web.Pages.Listings;

[Authorize]
public class EditModel(IQueryMediator queryMediator, ICommandMediator commandMediator, IImageStorageService imageStorageService, ILocationRepository locationRepository) : PageModel
{
    [BindProperty]
    public ListingInput Input { get; set; } = new();

    [BindProperty]
    [Display(Name = "Add Images")]
    public List<IFormFile> Images { get; set; } = [];

    [BindProperty]
    public List<string> RemoveImageUrls { get; set; } = [];

    public IReadOnlyList<string> CurrentImageUrls { get; set; } = [];
    public List<SelectListItem> Regions { get; set; } = [];

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

        await LoadRegionsAsync(cancellationToken);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadRegionsAsync(cancellationToken);
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
            await LoadRegionsAsync(cancellationToken);
            return Page();
        }

        return RedirectToPage("Index");
    }

    private async Task LoadRegionsAsync(CancellationToken cancellationToken)
    {
        var regions = await locationRepository.GetAllRegionsAsync(cancellationToken);
        Regions = regions.Select(r => new SelectListItem(r.Name, r.Id.ToString())).ToList();
    }

    public class ListingInput
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Заглавието е задължително.")]
        [StringLength(200, ErrorMessage = "Заглавието не може да бъде по-дълго от 200 символа.")]
        [Display(Name = "Заглавие")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Описанието е задължително.")]
        [StringLength(4000, ErrorMessage = "Описанието не може да бъде по-дълго от 4000 символа.")]
        [Display(Name = "Описание")]
        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Цената не може да бъде отрицателна.")]
        [Display(Name = "Цена (оставете празно за \"По договаряне\")")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Адресът е задължителен.")]
        [StringLength(300, ErrorMessage = "Адресът не може да бъде по-дълъг от 300 символа.")]
        [Display(Name = "Адрес")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Населеното място е задължително.")]
        [Display(Name = "Населено място")]
        public Guid CityId { get; set; }

        [Range(0, 100, ErrorMessage = "Броят спални трябва да бъде между 0 и 100.")]
        [Display(Name = "Спални")]
        public int NumberOfBedrooms { get; set; }

        [Range(0, 100, ErrorMessage = "Броят бани трябва да бъде между 0 и 100.")]
        [Display(Name = "Бани")]
        public int NumberOfBathrooms { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Квадратурата не може да бъде отрицателна.")]
        [Display(Name = "Квадратура (кв.м.)")]
        public double SquareFootage { get; set; }
    }
}
