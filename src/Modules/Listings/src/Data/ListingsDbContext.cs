using Listings.Listing.Entities;
using Locations.Location.Entities;
using Ratings.Rating.Entities;
using Comments.Comment.Entities;
using Microsoft.EntityFrameworkCore;

namespace Listings.Data;

public class ListingsDbContext(DbContextOptions<ListingsDbContext> options) : DbContext(options)
{
    public DbSet<Listing.Entities.Listing> Listings => Set<Listing.Entities.Listing>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<CommentAuditLog> CommentAuditLogs => Set<CommentAuditLog>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Region> Regions => Set<Region>();
    public DbSet<Municipality> Municipalities => Set<Municipality>();
    public DbSet<City> Cities => Set<City>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ListingsDbContext).Assembly);
    }
}
