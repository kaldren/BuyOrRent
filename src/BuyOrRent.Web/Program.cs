using System.Globalization;
using Listings.Data;
using Listings.Extensions;
using BuyOrRent.Web;
using BuyOrRent.Web.Extensions;
using BuyOrRent.Web.Settings;
using Comments.Comment.Abstractions;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<ThemeSettings>(builder.Configuration.GetSection(ThemeSettings.SectionName));

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Admin"));
});
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResource));
    });

// Register modules
builder.Services.AddListingsModule(builder.Configuration);

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ListingsDbContext>();
    if (app.Environment.IsDevelopment())
    {
        db.Database.EnsureDeleted();
    }
    db.Database.EnsureCreated();

    var seeder = scope.ServiceProvider.GetRequiredService<LocationSeeder>();
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

var supportedCultures = new[] { new CultureInfo("bg"), new CultureInfo("en") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("bg"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapControllers();
app.MapListingsModule();

app.MapPost("/api/notifications/mark-read", async (INotificationRepository repo, HttpContext ctx) =>
{
    var userId = ctx.User.GetUserId();
    if (userId == Guid.Empty)
        return Results.Unauthorized();
    await repo.MarkAllReadAsync(userId);
    return Results.Ok();
}).RequireAuthorization();

app.MapPost("/culture", ([FromForm] string culture, [FromForm] string returnUrl, HttpContext context) =>
{
    context.Response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
    );
    return Results.LocalRedirect(returnUrl ?? "/");
}).DisableAntiforgery();

app.Run();
