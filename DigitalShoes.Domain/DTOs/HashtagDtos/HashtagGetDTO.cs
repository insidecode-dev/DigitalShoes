using DigitalShoes.Domain.DTOs.ShoeDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.DTOs.HashtagDtos
{
    public class HashtagGetDTO
    {
        public string Text { get; set; }
        public List<ShoeGetDTO> ShoeGetDTO { get; set; }  
    }
}
