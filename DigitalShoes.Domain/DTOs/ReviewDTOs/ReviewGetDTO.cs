using DigitalShoes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DigitalShoes.Domain.StaticDetails;

namespace DigitalShoes.Domain.DTOs.ReviewDTOs
{
    public class ReviewGetDTO
    {
        public int Id { get; set; }
        public int ApplicationUserId { get; set; }        
        public int ShoeId { get; set; }
        public string ReviewText { get; set; }
        public Rating Rating { get; set; }
    }
}
