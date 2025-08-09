using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Petsgram.Domain.Entities;

namespace Petsgram.Infrastructure.EntityConfigurations;

public class PetPhotoConfiguration : IEntityTypeConfiguration<PetPhoto>
{
    public void Configure(EntityTypeBuilder<PetPhoto> builder)
    {
        builder.HasKey(pp => pp.Id);
        builder.Property(pp => pp.Path).IsRequired();
        builder.Property(pp => pp.PublicUrl).IsRequired();
        builder.HasOne(pp => pp.Pet)
            .WithMany(p => p.Photos)
            .HasForeignKey(pp => pp.PetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
