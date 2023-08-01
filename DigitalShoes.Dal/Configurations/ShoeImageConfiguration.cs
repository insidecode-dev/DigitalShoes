using DigitalShoes.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DigitalShoes.Dal.Configurations
{
    public class ShoeImageConfiguration : IEntityTypeConfiguration<ShoeImage>
    {
        public void Configure(EntityTypeBuilder<ShoeImage> builder)
        {
            //relation
            builder
                .HasOne(u => u.Shoe)
                .WithMany(c => c.ShoeImages)
                .HasForeignKey(c => c.ShoeId);

            builder
                .HasOne(u => u.Image)
                .WithMany(c => c.ShoeImages)
                .HasForeignKey(c => c.ImageId);

            //

            builder
                .Ignore(x => x.Id);

            builder.
                HasKey(sh => new { sh.ShoeId, sh.ImageId });
        }
    }
}
