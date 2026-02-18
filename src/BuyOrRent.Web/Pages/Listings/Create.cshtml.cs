using System.ComponentModel.DataAnnotations;
using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Application.Features.CreateListing;
using BuyOrRent.Modules.Listings.Domain.Entities;
using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuyOrRent.Web.Pages.Listings;

public class CreateModel(ICommandMediator commandMediator, IImageStorageService imageStorageService, ILocationRepository locationRepository) : PageModel
{
    [BindProperty]
    public ListingInput Input { get; set; } = new();

    [BindProperty]
    [Display(Name = "Listing Images")]
    public List<IFormFile> Images { get; set; } = [];

    public List<SelectListItem> Countries { get; set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadCountriesAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadCountriesAsync(cancellationToken);
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
            SellerId = Guid.NewGuid(), // Placeholder until auth provides real user ID
            ImageUrls = imageUrls
        };

        await commandMediator.SendAsync(command, cancellationToken);

        return RedirectToPage("Index");
    }

    private async Task LoadCountriesAsync(CancellationToken cancellationToken)
    {
        var countries = await locationRepository.GetCountriesAsync(cancellationToken);
        Countries = countries.Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList();
    }

    public class ListingInput
    {
        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(4000)]
        public string Description { get; set; } = string.Empty;

        [Required, Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required, StringLength(300)]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Country")]
        public Guid? CountryId { get; set; }

        [Display(Name = "Region")]
        public Guid? RegionId { get; set; }

        [Display(Name = "Municipality")]
        public Guid? MunicipalityId { get; set; }

        [Required, Display(Name = "City")]
        public Guid CityId { get; set; }

        [Required, StringLength(20), Display(Name = "Zip Code")]
        public string ZipCode { get; set; } = string.Empty;

        [Range(0, 100), Display(Name = "Bedrooms")]
        public int NumberOfBedrooms { get; set; }

        [Range(0, 100), Display(Name = "Bathrooms")]
        public int NumberOfBathrooms { get; set; }

        [Range(0, double.MaxValue), Display(Name = "Square Footage")]
        public double SquareFootage { get; set; }
    }
}
