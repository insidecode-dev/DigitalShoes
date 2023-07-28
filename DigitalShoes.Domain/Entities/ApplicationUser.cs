using Microsoft.AspNetCore.Identity;

namespace DigitalShoes.Domain.Entities
{    
    public class ApplicationUser:IdentityUser<int>
    {        
        public string Name { get; set; }
        //public Image Image { get; set; }        
        public string ImageUrl { get; set; }
        public string ImageLocalPath { get; set; }
        public Cart Cart { get; set; }                
        public Wishlist Wishlist { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Payment> Payments { get; set; }
    }
}
