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
    internal class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            //one-to-many
            builder
                .HasOne(u => u.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(i => i.CartId);

            builder
                .HasOne(s => s.Shoe)
                .WithMany(c => c.CartItems)
                .HasForeignKey(i => i.ShoeId);

            //

            builder
                .Property(a => a.ItemsCount)
                .IsRequired()
                .HasColumnType("int");

            builder
                .Property(a => a.Price)
                .IsRequired()
                .HasColumnType("decimal(6, 2)");

        }
    }
}