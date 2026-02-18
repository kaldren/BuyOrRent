using BuyOrRent.Modules.Listings.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuyOrRent.Modules.Listings.Infrastructure.Persistence;

public class ListingsDbContext(DbContextOptions<ListingsDbContext> options) : DbContext(options)
{
    public DbSet<Listing> Listings => Set<Listing>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ListingsDbContext).Assembly);
    }
}
