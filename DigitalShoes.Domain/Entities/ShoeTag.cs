namespace DigitalShoes.Domain.Entities
{
    public class ShoeTag : BaseEntity // Ignore id for ShoeTag
    {
        public int ShoeId { get; set; }
        public Shoe Shoe { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}