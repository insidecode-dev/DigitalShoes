using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public int Id { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DataStatus DataStatus { get; set; }
    }
}
