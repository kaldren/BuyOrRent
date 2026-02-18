using System.ComponentModel.DataAnnotations;
using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Application.Features.CreateListing;
using BuyOrRent.Modules.Listings.Domain.Exceptions;
using BuyOrRent.Web.Extensions;
using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuyOrRent.Web.Pages.Listings;

[Authorize]
public class CreateModel(ICommandMediator commandMediator, IImageStorageService imageStorageService, ILocationRepository locationRepository) : PageModel
{
    [BindProperty]
    public ListingInput Input { get; set; } = new();

    [BindProperty]
    [Display(Name = "Listing Images")]
    public List<IFormFile> Images { get; set; } = [];

    public List<SelectListItem> Regions { get; set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadRegionsAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadRegionsAsync(cancellationToken);
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
            ZipCode = Input.ZipCode,
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

        [Display(Name = "Област")]
        public Guid? RegionId { get; set; }

        [Display(Name = "Община")]
        public Guid? MunicipalityId { get; set; }

        [Required(ErrorMessage = "Населеното място е задължително.")]
        [Display(Name = "Населено място")]
        public Guid CityId { get; set; }

        [Required(ErrorMessage = "Пощенският код е задължителен.")]
        [StringLength(20, ErrorMessage = "Пощенският код не може да бъде по-дълъг от 20 символа.")]
        [Display(Name = "Пощенски код")]
        public string ZipCode { get; set; } = string.Empty;

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
