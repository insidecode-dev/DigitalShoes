using DigitalShoes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DigitalShoes.Domain.StaticDetails;

namespace DigitalShoes.Domain.DTOs.ShoeDTOs
{
    public class ShoeDTO
    {
        public ShoeDTO()
        {
            Hashtags = new List<string>();
        }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Count { get; set; }
        public int Size { get; set; }
        public string Description { get; set; }
        public Gender Gender { get; set; }
        public Color Color { get; set; }
        public int CategoryId { get; set; }
        public int ApplicationUserId { get; set; }
        public List<string> Hashtags { get; set; }
    }
}
