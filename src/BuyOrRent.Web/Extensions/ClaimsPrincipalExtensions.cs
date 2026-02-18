using System.Security.Claims;

namespace BuyOrRent.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        // Microsoft Entra ID provides the object ID via the "oid" claim
        // or "http://schemas.microsoft.com/identity/claims/objectidentifier"
        var oid = principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")
               ?? principal.FindFirstValue("oid");

        if (Guid.TryParse(oid, out var userId))
            return userId;

        // Fallback to NameIdentifier for other identity providers
        var nameId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(nameId, out userId))
            return userId;

        return Guid.Empty;
    }
}
