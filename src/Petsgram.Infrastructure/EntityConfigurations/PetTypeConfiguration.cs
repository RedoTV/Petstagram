using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Petsgram.Domain.Entities;

namespace Petsgram.Infrastructure.EntityConfigurations;

public class PetTypeConfiguration : IEntityTypeConfiguration<PetType>
{
    public void Configure(EntityTypeBuilder<PetType> builder)
    {
        builder.HasKey(pt => pt.Id);
        builder.Property(pt => pt.Name).IsRequired();
        builder.HasMany(pt => pt.Pets)
            .WithOne(p => p.PetType)
            .HasForeignKey(p => p.PetTypeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
