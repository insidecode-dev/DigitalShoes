using DigitalShoes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DigitalShoes.Domain.StaticDetails;

namespace DigitalShoes.Domain.DTOs.SearchDTOs
{
    public class GetShoeByFilterDTO
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Count { get; set; } = 10;
        public int Size { get; set; }
        public string Rating { get; set; }
        public decimal Price { get; set; }        
        public string Gender { get; set; }        
        public string Color { get; set; }
        public string CTName { get; set; }        
    }
}
