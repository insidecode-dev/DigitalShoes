using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static DigitalShoes.Domain.StaticDetails;

namespace DigitalShoes.Domain.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            CreatedDate = DateTime.Now;
            DataStatus = DataStatus.Inserted;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }        
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DataStatus DataStatus { get; set; }
    }
}
