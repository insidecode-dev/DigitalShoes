using DigitalShoes.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace DigitalShoes.Dal.Configurations
{
    public class ShoeHashtagConfiguartion : IEntityTypeConfiguration<ShoeHashtag>
    {
        public void Configure(EntityTypeBuilder<ShoeHashtag> builder)
        {
            //relation
            builder
                .HasOne(u => u.Shoe)
                .WithMany(c => c.ShoeHashtags)
                .HasForeignKey(c => c.ShoeId);

            builder
                .HasOne(u => u.Hashtag)
                .WithMany(c => c.ShoeHashtags)
                .HasForeignKey(c => c.HashtagId);
            //

            builder
                .Ignore(x => x.Id);

            builder.
                HasKey(sh => new { sh.ShoeId, sh.HashtagId });
        }
    }
}