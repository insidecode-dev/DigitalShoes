namespace DigitalShoes.Domain.Entities
{
    public class PaymentObject:BaseEntity
    {
        public int ShoeId { get; set; }        
        public int PaymentId { get; set; } // where is shoe
        public Payment Payment { get; set; }
        public int ItemsCount { get; set; }
        public decimal Price { get; set; }
    }
}