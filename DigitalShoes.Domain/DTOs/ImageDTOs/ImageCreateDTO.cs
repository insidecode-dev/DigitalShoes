using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.DTOs.ImageDTOs
{
    public class ImageCreateDTO
    {
        public List<IFormFile> Image { get; set; }
        public int ShoeId { get; set; }
    }
}
