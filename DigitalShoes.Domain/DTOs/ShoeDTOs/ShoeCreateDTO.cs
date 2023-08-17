using DigitalShoes.Domain.DTOs.HashtagDtos;
using DigitalShoes.Domain.DTOs.ImageDTOs;
using DigitalShoes.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DigitalShoes.Domain.StaticDetails;

namespace DigitalShoes.Domain.DTOs.ShoeDTOs
{
    public class ShoeCreateDTO
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Count { get; set; }
        public int Size { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Gender { get; set; }
        public string Color { get; set; }
        public string CTName { get; set; }                
        public List<HashtagDTO> Hashtags { get; set; }                
    }
}
