using DigitalShoes.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DigitalShoes.Dal.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // relation
            builder
                .HasOne(c => c.Cart)
                .WithOne(u => u.ApplicationUser)
                .HasForeignKey<Cart>(x => x.ApplicationUserId);

            builder
                .HasOne(c => c.Wishlist)
                .WithOne(u => u.ApplicationUser)
                .HasForeignKey<Wishlist>(x => x.ApplicationUserId);

            builder
                .HasMany(c => c.Reviews)
                .WithOne(u => u.ApplicationUser)
                .HasForeignKey(x => x.ApplicationUserId);

            builder
                .HasMany(c => c.Payments)
                .WithOne(u => u.ApplicationUser)
                .HasForeignKey(x => x.ApplicationUserId);

            builder
                .HasMany(c => c.Shoes)
                .WithOne(u => u.ApplicationUser)
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .Property(a => a.Name)
                .IsRequired()
                .HasColumnType("nvarchar(20)");

            builder
                .Property(a => a.Balance)
                .IsRequired()
                .HasColumnType("decimal(6, 2)")
                .HasDefaultValue(0.00M);

            builder
                .Property(a => a.ProfileImageUrl)
                .IsRequired(false)
                .HasColumnType("nvarchar(200)");

            builder
                .Property(a => a.ProfileImageLocalPath)
                .IsRequired(false)
                .HasColumnType("nvarchar(200)");

            builder
                .Property(a => a.OrderAdress)
                .IsRequired()
                .HasColumnType("nvarchar(500)");            
        }
    }
}
