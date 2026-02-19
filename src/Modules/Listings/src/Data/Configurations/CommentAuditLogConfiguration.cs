using Comments.Comment.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Listings.Data.Configurations;

public class CommentAuditLogConfiguration : IEntityTypeConfiguration<CommentAuditLog>
{
    public void Configure(EntityTypeBuilder<CommentAuditLog> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(l => l.AdminDisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.OldContent)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(l => l.NewContent)
            .HasMaxLength(2000);

        builder.Property(l => l.CommentAuthorName)
            .IsRequired()
            .HasMaxLength(200);
    }
}
