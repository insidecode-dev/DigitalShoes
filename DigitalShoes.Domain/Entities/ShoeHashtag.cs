namespace DigitalShoes.Domain.Entities
{
    public class ShoeHashtag : BaseEntity // Ignore id for ShoeTag
    {
        public int ShoeId { get; set; }
        public Shoe Shoe { get; set; }
        public int HashtagId { get; set; }
        public Hashtag Hashtag { get; set; }
    }
}