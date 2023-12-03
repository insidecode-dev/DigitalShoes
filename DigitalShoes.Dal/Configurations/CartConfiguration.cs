using DigitalShoes.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace DigitalShoes.Dal.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            //one-to-many
            builder
                .HasOne(u => u.ApplicationUser)
                .WithOne(c => c.Cart)
                .HasForeignKey<Cart>(i => i.ApplicationUserId);

            builder
                .HasMany(ci => ci.CartItems)
                .WithOne(c => c.Cart)
                .HasForeignKey(i => i.CartId);

            //
            builder
                .Property(a => a.ItemsCount)
                .IsRequired()
                .HasColumnType("int")
                .HasDefaultValue(0);

            builder
                .Property(a => a.TotalPrice)
                .IsRequired()
                .HasColumnType("decimal(6, 2)")
                .HasDefaultValue(0.00M);
        }
    }
}
