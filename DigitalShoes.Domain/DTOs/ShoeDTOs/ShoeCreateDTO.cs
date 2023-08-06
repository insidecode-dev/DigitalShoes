using DigitalShoes.Domain.DTOs.HashtagDtos;
using DigitalShoes.Domain.DTOs.ImageDTOs;
using DigitalShoes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DigitalShoes.Domain.StaticDetails;

namespace DigitalShoes.Domain.DTOs.ProductDTOs
{
    public class ShoeCreateDTO
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Count { get; set; }
        public int Size { get; set; }
        public string Description { get; set; }
        public Gender Gender { get; set; }
        public Color Color { get; set; }
        public int CategoryName { get; set; }                
        public List<HashtagDTO> Hashtags { get; set; }
        public List<ImageCreateDTO> _Images { get; set; }

    }
}
