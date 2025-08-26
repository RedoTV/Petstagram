using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Petsgram.Domain.Entities;

namespace Petsgram.Infrastructure.EntityConfigurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Token).IsRequired();
        builder.Property(rt => rt.CreatedAt).IsRequired();
        builder.Property(rt => rt.ExpiresAt).IsRequired();
        builder.Property(rt => rt.IsRevoked).IsRequired();
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
