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
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            //relation
            builder
                .HasOne(u => u.ApplicationUser)
                .WithMany(c => c.Reviews)
                .HasForeignKey(c => c.ApplicationUserId);

            builder
                .HasOne(u => u.Shoe)
                .WithMany(c => c.Reviews)
                .HasForeignKey(c => c.ShoeId);
            //

            builder
                .Property(a => a.ReviewText)
                .IsRequired()
                .HasColumnType("nvarchar(200)");
        }
    }
}
