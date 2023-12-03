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
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            //relation
            builder
                .HasOne(u => u.Shoe)
                .WithMany(c => c.Images)
                .HasForeignKey(c => c.ShoeId)
                .OnDelete(DeleteBehavior.NoAction);

            //

            builder
                .Property(n => n.ImageUrl)
                .IsRequired()
                .HasColumnType("nvarchar(200)");

            builder
                .Property(n => n.ImageLocalPath)
                .IsRequired()
                .HasColumnType("nvarchar(200)");
        }
    }
}
