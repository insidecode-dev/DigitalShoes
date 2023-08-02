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
    public class ShoeConfiguration : IEntityTypeConfiguration<Shoe>
    {
        public void Configure(EntityTypeBuilder<Shoe> builder)
        {
            //relation

            builder
                .HasOne(u => u.Category)
                .WithMany(c => c.Shoes)
                .HasForeignKey(c => c.CategoryId);

            builder
                .HasOne(u => u.ApplicationUser)
                .WithMany(c => c.Shoes)
                .HasForeignKey(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(u => u.Images)
                .WithOne(c => c.Shoe)
                .HasForeignKey(c => c.ShoeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(u => u.Reviews)
                .WithOne(c => c.Shoe)
                .HasForeignKey(c => c.ShoeId);

            builder
                .HasMany(u => u.ShoeWishlists)
                .WithOne(c => c.Shoe)
                .HasForeignKey(c => c.ShoeId);

            builder
                .HasMany(u => u.CartItems)
                .WithOne(c => c.Shoe)
                .HasForeignKey(c => c.ShoeId);

            builder
                .HasMany(u => u.PaymentObjects)
                .WithOne(c => c.Shoe)
                .HasForeignKey(c => c.ShoeId);

            //

            builder
                .Property(a => a.Brand)
                .IsRequired()
                .HasColumnType("nvarchar(40)");

            builder
                .Property(a => a.Model)
                .IsRequired()
                .HasColumnType("nvarchar(40)");

            builder
                .Property(a => a.Count)
                .IsRequired()
                .HasColumnType("int");

            builder
                .Property(a => a.Size)
                .IsRequired()
                .HasColumnType("int");

            builder
                .Property(a => a.Description)
                .IsRequired()
                .HasColumnType("nvarchar(200)");
        }
    }
}
