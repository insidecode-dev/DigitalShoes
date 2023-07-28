namespace DigitalShoes.Domain.Entities
{
    public class CartItem:BaseEntity
    {        
        public int ShoeId { get; set; }
        public Shoe Shoe { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
    }
}