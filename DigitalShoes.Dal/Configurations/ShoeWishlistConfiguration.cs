using DigitalShoes.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Dal.Configurations
{
    public class ShoeWishlistConfiguration : IEntityTypeConfiguration<ShoeWishlist>
    {
        public void Configure(EntityTypeBuilder<ShoeWishlist> builder)
        {
            //relation
            builder
                .HasOne(u => u.Shoe)
                .WithMany(c => c.ShoeWishlists)
                .HasForeignKey(c => c.ShoeId);

            builder
                .HasOne(u => u.Wishlist)
                .WithMany(c => c.DesiredShoes)
                .HasForeignKey(c => c.WishlistId);

            //

            builder
                .Ignore(x => x.Id);

            builder.
                HasKey(sh => new { sh.ShoeId, sh.WishlistId });
        }
    }
}
