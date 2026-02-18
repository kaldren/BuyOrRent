using System.ComponentModel.DataAnnotations;
using Listings.Listing.Dtos;
using Listings.Listing.Enums;
using Listings.Listing.Features.ApproveListing;
using Listings.Listing.Features.GetListing;
using Listings.Listing.Features.RejectListing;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuyOrRent.Web.Pages.Admin;

[Authorize(Policy = "Admin")]
public class ReviewListingModel(IQueryMediator queryMediator, ICommandMediator commandMediator) : PageModel
{
    public ListingDto Listing { get; set; } = default!;

    [BindProperty]
    [StringLength(2000, ErrorMessage = "Причината не може да бъде по-дълга от 2000 символа.")]
    [Display(Name = "Причина за отхвърляне")]
    public string? RejectionReason { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var listing = await queryMediator.QueryAsync(new GetListingQuery { Id = id }, cancellationToken);
        if (listing is null)
            return NotFound();

        if (listing.Status != ListingStatus.PendingReview)
            return RedirectToPage("ReviewQueue");

        Listing = listing;
        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await commandMediator.SendAsync(new ApproveListingCommand { Id = id }, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToPage("ReviewQueue");
    }

    public async Task<IActionResult> OnPostRejectAsync(Guid id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(RejectionReason))
        {
            ModelState.AddModelError(nameof(RejectionReason), "Причината за отхвърляне е задължителна.");
            var listing = await queryMediator.QueryAsync(new GetListingQuery { Id = id }, cancellationToken);
            if (listing is null)
                return NotFound();
            Listing = listing;
            return Page();
        }

        try
        {
            await commandMediator.SendAsync(new RejectListingCommand
            {
                Id = id,
                Reason = RejectionReason
            }, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToPage("ReviewQueue");
    }
}
