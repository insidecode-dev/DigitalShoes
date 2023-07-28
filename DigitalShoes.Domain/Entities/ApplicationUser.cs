using Microsoft.AspNetCore.Identity;

namespace DigitalShoes.Domain.Entities
{    
    public class ApplicationUser:IdentityUser<int>
    {        
        public string Name { get; set; }
        public int ProfileImageId { get; set; }
        public Image Image { get; set; }
        public int CartId { get; set; }  
        public Cart Cart { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
