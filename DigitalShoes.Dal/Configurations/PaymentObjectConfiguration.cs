using DigitalShoes.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DigitalShoes.Dal.Configurations
{
    public class PaymentObjectConfiguration : IEntityTypeConfiguration<PaymentObject>
    {
        public void Configure(EntityTypeBuilder<PaymentObject> builder)
        {
            //relation
            builder
                .HasOne(u => u.Payment)
                .WithMany(c => c.PaymentObjects)
                .HasForeignKey(c => c.PaymentId);

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