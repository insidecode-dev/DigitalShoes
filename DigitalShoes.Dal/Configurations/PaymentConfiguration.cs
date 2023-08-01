using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalShoes.Domain.Entities;

namespace DigitalShoes.Dal.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            //relation
            builder
                .HasOne(u => u.ApplicationUser)
                .WithMany(c => c.Payments)
                .HasForeignKey(c => c.ApplicationUserId);

            builder
                .HasMany(u => u.PaymentObjects)
                .WithOne(c => c.Payment)
                .HasForeignKey(c => c.PaymentId);
            //

            builder
                .Property(a => a.TotalPrice)
                .IsRequired()
                .HasColumnType("decimal(6, 2)");
        }
    }
}
