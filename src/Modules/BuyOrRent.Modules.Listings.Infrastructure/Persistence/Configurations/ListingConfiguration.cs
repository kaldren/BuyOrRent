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

        builder.HasOne(l => l.City)
            .WithMany()
            .HasForeignKey(l => l.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(l => l.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(l => l.RejectionReason)
            .HasMaxLength(2000);

        builder.PrimitiveCollection(l => l.ImageUrls)
            .HasColumnType("nvarchar(max)");
    }
}
