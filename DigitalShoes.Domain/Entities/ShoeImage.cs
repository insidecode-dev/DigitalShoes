using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.Entities
{
    public class ShoeImage:BaseEntity // Ignore id for ShoeImage
    {
        public int ShoeId { get; set; }    
        public Shoe Shoe { get; set; }
        public int ImageId { get; set; }
        public Image Image { get; set; }
    }
}
