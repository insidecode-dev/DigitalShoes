using DigitalShoes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DigitalShoes.Domain.DTOs.ReviewDTOs
{
    public class ReviewCreateDTO
    {        
        public int ShoeId { get; set; }     
        public string ReviewText { get; set; }
        public string Rating { get; set; }
    }
}
