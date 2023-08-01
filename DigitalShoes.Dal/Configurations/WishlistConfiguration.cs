using DigitalShoes.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DigitalShoes.Dal.Configurations
{
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            //relation
            builder
                .HasOne(u => u.ApplicationUser)
                .WithOne(c => c.Wishlist)
                .HasForeignKey<Wishlist>(c => c.ApplicationUserId);

            builder
                .HasMany(u => u.DesiredShoes)
                .WithOne(c => c.Wishlist)
                .HasForeignKey(c => c.WishlistId);
        }
    }
}
