namespace DigitalShoes.Domain.Entities
{
    public class PaymentObject:BaseEntity
    {
        public int ShoeId { get; set; }
        public Shoe Shoe { get; set;}        
        public int PaymentId { get; set; }
        public Payment Payment { get; set; }
        public int ItemsCount { get; set; }
        public decimal Price { get; set; }
    }
}