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
    public class HashtagConfiguration : IEntityTypeConfiguration<Hashtag>
    {
        public void Configure(EntityTypeBuilder<Hashtag> builder)
        {
            //relation
            builder
                .HasMany(u => u.ShoeHashtags)
                .WithOne(c => c.Hashtag)
                .HasForeignKey(c => c.HashtagId);

            //

            builder
                .Property(a => a.Text)
                .IsRequired()
                .HasColumnType("nvarchar(20)");
        }
    }
}
