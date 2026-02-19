using BuyOrRent.Web.Extensions;
using Comments.Comment.Abstractions;
using Comments.Comment.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BuyOrRent.Web.ViewComponents;

public class NotificationBellViewComponent(INotificationRepository notificationRepository) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (UserClaimsPrincipal.Identity?.IsAuthenticated != true)
            return Content(string.Empty);

        var userId = UserClaimsPrincipal.GetUserId();
        if (userId == Guid.Empty)
            return Content(string.Empty);

        var unreadCount = await notificationRepository.GetUnreadCountAsync(userId);
        var recent = await notificationRepository.GetRecentUnreadAsync(userId, 10);

        return View(new NotificationBellViewModel
        {
            UnreadCount = unreadCount,
            Notifications = recent
        });
    }
}

public class NotificationBellViewModel
{
    public int UnreadCount { get; set; }
    public IReadOnlyList<Notification> Notifications { get; set; } = [];
}
