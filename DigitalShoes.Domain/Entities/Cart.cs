using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.Entities
{
    public class Cart:BaseEntity
    {
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public List<CartItem> CartItems { get; set; }
        public int ItemsCount { get; set; }
        public decimal TotalPrice { get; set; }   
    }
}
