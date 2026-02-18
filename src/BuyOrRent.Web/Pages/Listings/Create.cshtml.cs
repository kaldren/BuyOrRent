using System.ComponentModel.DataAnnotations;
using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Application.Features.CreateListing;
using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuyOrRent.Web.Pages.Listings;

public class CreateModel(ICommandMediator commandMediator, IImageStorageService imageStorageService) : PageModel
{
    [BindProperty]
    public ListingInput Input { get; set; } = new();

    [BindProperty]
    [Display(Name = "Listing Images")]
    public List<IFormFile> Images { get; set; } = [];

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return Page();

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
            City = Input.City,
            State = Input.State,
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

        [Required, StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string State { get; set; } = string.Empty;

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
