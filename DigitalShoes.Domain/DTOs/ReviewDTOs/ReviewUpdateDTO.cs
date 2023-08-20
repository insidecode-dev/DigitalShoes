using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.DTOs.ReviewDTOs
{
    public class ReviewUpdateDTO
    {
        public int ReviewId { get; set; }
        public int ShoeId { get; set; }
        public string ReviewText { get; set; }
        public string Rating { get; set; }
    }
}
