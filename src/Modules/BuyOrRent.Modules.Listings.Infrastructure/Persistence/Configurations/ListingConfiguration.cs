using BuyOrRent.Modules.Listings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuyOrRent.Modules.Listings.Infrastructure.Persistence.Configurations;

public class ListingConfiguration : IEntityTypeConfiguration<Listing>
{
    public void Configure(EntityTypeBuilder<Listing> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Description)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(l => l.Price)
            .HasPrecision(18, 2);

        builder.Property(l => l.Address)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(l => l.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.State)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.ZipCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(l => l.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.PrimitiveCollection(l => l.ImageUrls)
            .HasColumnType("nvarchar(max)");
    }
}
