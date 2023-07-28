using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.Entities
{
    public class Wishlist : BaseEntity 
    {        
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public List<ShoeWishlist> DesiredShoes { get; set; }
    }
}
