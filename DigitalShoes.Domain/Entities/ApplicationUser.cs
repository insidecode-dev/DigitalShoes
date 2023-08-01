using Microsoft.AspNetCore.Identity;

namespace DigitalShoes.Domain.Entities
{    
    public class ApplicationUser:IdentityUser<int>
    {
        public string Name { get; set; }        
        public string ProfileImageUrl { get; set; }
        public string ProfileImageLocalPath { get; set; }
        public Cart Cart { get; set; }                
        public Wishlist Wishlist { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Payment> Payments { get; set; }
        public decimal Balance { get; set; }
    }
}
