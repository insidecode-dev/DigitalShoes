using DigitalShoes.Dal.Configurations;
using DigitalShoes.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DigitalShoes.Dal.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Hashtag> Hashtags { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentObject> PaymentObjects { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Shoe> Shoes { get; set; }
        public DbSet<ShoeHashtag> ShoeHashtags { get; set; }        
        public DbSet<ShoeWishlist> ShoeWishlists { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }                      
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
