using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Petsgram.Domain.Entities;

namespace Petsgram.Infrastructure.EntityConfigurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasOne(p => p.User)
            .WithMany(u => u.Pets)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.PetType)
            .WithMany(pt => pt.Pets)
            .HasForeignKey(p => p.PetTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Photos)
            .WithOne(pp => pp.Pet)
            .HasForeignKey(pp => pp.PetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
