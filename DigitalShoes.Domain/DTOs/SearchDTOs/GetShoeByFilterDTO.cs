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
        public string? Brand { get; set; } = null;
        public string? Model { get; set; } = null;
        public int Count { get; set; } = 10;
        public int? Size { get; set; } = null;
        public string? Rating { get; set; } = null;
        public decimal? Price { get; set; } = null;
        public string? Gender { get; set; } = null;        
        public string? Color { get; set; } = null;
        public string? CTName { get; set; } = null;        
    }
}
