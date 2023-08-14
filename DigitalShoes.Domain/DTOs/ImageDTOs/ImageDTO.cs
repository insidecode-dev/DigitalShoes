using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.DTOs.ImageDTOs
{
    public class ImageDTO
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string ImageLocalPath { get; set; }
        public int ShoeId { get; set; }
    }
}
