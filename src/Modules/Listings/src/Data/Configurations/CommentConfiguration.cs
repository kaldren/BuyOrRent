using Comments.Comment.Entities;
using Listings.Listing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Listings.Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.UserDisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.OwnerReply)
            .HasMaxLength(2000);

        builder.Property(c => c.OwnerReplyDisplayName)
            .HasMaxLength(200);

        builder.HasOne<Listing.Entities.Listing>()
            .WithMany()
            .HasForeignKey(c => c.ListingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => new { c.ListingId, c.UserId })
            .IsUnique();
    }
}
