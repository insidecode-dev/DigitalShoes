using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.DTOs.CartItemDTOs
{
    public class CartItemGetDTO
    {
        public int Id { get; set; }
        public int ShoeId { get; set; }
        public int ItemsCount { get; set; }        
        public decimal Price { get; set; }
    }
}
