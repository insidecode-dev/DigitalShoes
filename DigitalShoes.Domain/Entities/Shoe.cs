using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DigitalShoes.Domain.StaticDetails;

namespace DigitalShoes.Domain.Entities
{
    public class Shoe : BaseEntity
    {        
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Count { get; set; }
        public int Size { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public Gender Gender { get; set; }
        public Rating Rating { get; set; }
        public Color Color { get; set; }
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public List<Image> Images { get; set; }        
        public List<ShoeHashtag> ShoeHashtags { get; set; }
        public List<ShoeWishlist> ShoeWishlists { get; set; }
        public List<CartItem> CartItems { get; set; }
        public List<Review> Reviews { get; set; }
        public List<PaymentObject> PaymentObjects { get; set; }
    }
}