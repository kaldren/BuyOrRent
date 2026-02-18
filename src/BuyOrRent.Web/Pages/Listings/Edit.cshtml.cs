using System.ComponentModel.DataAnnotations;
using BuyOrRent.Modules.Listings.Application.Abstractions;
using BuyOrRent.Modules.Listings.Application.Features.EditListing;
using BuyOrRent.Modules.Listings.Application.Features.GetListing;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuyOrRent.Web.Pages.Listings;

public class EditModel(IQueryMediator queryMediator, ICommandMediator commandMediator, IImageStorageService imageStorageService) : PageModel
{
    [BindProperty]
    public ListingInput Input { get; set; } = new();

    [BindProperty]
    [Display(Name = "Add Images")]
    public List<IFormFile> Images { get; set; } = [];

    [BindProperty]
    public List<string> RemoveImageUrls { get; set; } = [];

    public IReadOnlyList<string> CurrentImageUrls { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var listing = await queryMediator.QueryAsync(new GetListingQuery { Id = id }, cancellationToken);
        if (listing is null)
            return NotFound();

        Input = new ListingInput
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            Price = listing.Price,
            Address = listing.Address,
            City = listing.City,
            State = listing.State,
            ZipCode = listing.ZipCode,
            NumberOfBedrooms = listing.NumberOfBedrooms,
            NumberOfBathrooms = listing.NumberOfBathrooms,
            SquareFootage = listing.SquareFootage
        };
        CurrentImageUrls = listing.ImageUrls;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return Page();

        var existing = await queryMediator.QueryAsync(new GetListingQuery { Id = Input.Id }, cancellationToken);
        if (existing is null)
            return NotFound();

        // Start with existing images, remove any checked for removal
        var imageUrls = existing.ImageUrls.ToList();
        foreach (var urlToRemove in RemoveImageUrls)
        {
            await imageStorageService.DeleteAsync(urlToRemove, cancellationToken);
            imageUrls.Remove(urlToRemove);
        }

        // Upload new images
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
            City = Input.City,
            State = Input.State,
            ZipCode = Input.ZipCode,
            NumberOfBedrooms = Input.NumberOfBedrooms,
            NumberOfBathrooms = Input.NumberOfBathrooms,
            SquareFootage = Input.SquareFootage,
            ImageUrls = imageUrls
        };

        try
        {
            await commandMediator.SendAsync(command, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToPage("Index");
    }

    public class ListingInput
    {
        [Required]
        public Guid Id { get; set; }

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
