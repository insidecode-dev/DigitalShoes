using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.Entities
{
    public class Image : BaseEntity
    {        
        public string ImageUrl { get; set; }
        public string ImageLocalPath { get; set; }
        public int ShoeId { get; set; }
        public Shoe Shoe{ get; set; }
    }
}
