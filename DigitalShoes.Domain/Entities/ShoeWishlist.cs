namespace DigitalShoes.Domain.Entities
{
    public class ShoeWishlist : BaseEntity // Ignore id for ShoeWishlist
    {
        public int ShoeId { get; set; }
        public Shoe Shoe { get; set; }
        public int WishlistId { get; set; }
        public Wishlist Wishlist { get; set; }
    }
}