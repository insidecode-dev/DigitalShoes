using DigitalShoes.Domain.DTOs.ImageDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DigitalShoes.Domain.StaticDetails;

namespace DigitalShoes.Domain.DTOs.ShoeDTOs
{
    public class ShoeGetDTO
    {
        public ShoeGetDTO()
        {
            Hashtag = new List<string>();
        }
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Count { get; set; }
        public int Size { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public Gender Gender { get; set; }
        public Color Color { get; set; }
        public int CategoryId { get; set; }        
        public List<string> Hashtag { get; set; }
        public List<ImageDTO> Images { get; set; }
    }
}
