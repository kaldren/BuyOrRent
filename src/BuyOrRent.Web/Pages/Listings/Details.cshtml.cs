using Listings.Listing.Dtos;
using Listings.Listing.Enums;
using Listings.Listing.Exceptions;
using Listings.Listing.Features.GetListing;
using Ratings.Rating.Features.GetListingRatingSummary;
using Ratings.Rating.Features.RateListing;
using Ratings.Rating.Dtos;
using Comments.Comment.Dtos;
using Comments.Comment.Features.AddComment;
using Comments.Comment.Features.GetListingComments;
using Comments.Comment.Features.ReplyToComment;
using BuyOrRent.Web.Extensions;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuyOrRent.Web.Pages.Listings;

public class DetailsModel(IQueryMediator queryMediator, ICommandMediator commandMediator) : PageModel
{
    public ListingDto Listing { get; set; } = default!;
    public RatingSummaryDto RatingSummary { get; set; } = new(0, 0, null);
    public PaginatedResult<CommentDto> Comments { get; set; } = new([], 0, 1, 5);
    public int CommentPage { get; set; } = 1;

    [BindProperty]
    public string? NewComment { get; set; }

    [BindProperty]
    public string? ReplyContent { get; set; }

    [BindProperty]
    public Guid? ReplyCommentId { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id, int commentPage, CancellationToken cancellationToken)
    {
        var listing = await queryMediator.QueryAsync(new GetListingQuery { Id = id }, cancellationToken);
        if (listing is null)
            return NotFound();

        // Non-active listings are only visible to the owner or admins
        if (listing.Status != ListingStatus.Active)
        {
            var userId = User.GetUserId();
            if (listing.SellerId != userId && !User.IsAdmin())
                return NotFound();
        }

        Listing = listing;

        RatingSummary = await queryMediator.QueryAsync(new GetListingRatingSummaryQuery
        {
            ListingId = id,
            CurrentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : null
        }, cancellationToken);

        CommentPage = commentPage < 1 ? 1 : commentPage;
        Comments = await queryMediator.QueryAsync(new GetListingCommentsQuery
        {
            ListingId = id,
            Page = CommentPage,
            PageSize = 5
        }, cancellationToken);

        return Page();
    }

    public async Task<IActionResult> OnPostRateAsync(Guid id, int stars, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true)
            return Challenge();

        try
        {
            await commandMediator.SendAsync(new RateListingCommand
            {
                ListingId = id,
                UserId = User.GetUserId(),
                Stars = stars
            }, cancellationToken);
        }
        catch (ListingDomainException)
        {
            // Silently ignore domain errors (e.g. self-rating)
        }
        catch (ArgumentException)
        {
            // Silently ignore validation errors (e.g. invalid rating)
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostCommentAsync(Guid id, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true)
            return Challenge();

        try
        {
            await commandMediator.SendAsync(new AddCommentCommand
            {
                ListingId = id,
                UserId = User.GetUserId(),
                UserDisplayName = User.Identity.Name ?? "Unknown",
                Content = NewComment ?? string.Empty
            }, cancellationToken);
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (KeyNotFoundException)
        {
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostReplyAsync(Guid id, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true)
            return Challenge();

        if (ReplyCommentId is null)
            return RedirectToPage(new { id });

        try
        {
            await commandMediator.SendAsync(new ReplyToCommentCommand
            {
                CommentId = ReplyCommentId.Value,
                UserId = User.GetUserId(),
                UserDisplayName = User.Identity.Name ?? "Unknown",
                ReplyContent = ReplyContent ?? string.Empty
            }, cancellationToken);
        }
        catch (ArgumentException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (KeyNotFoundException)
        {
        }

        return RedirectToPage(new { id });
    }
}
