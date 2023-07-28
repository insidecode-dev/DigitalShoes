using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DigitalShoes.Domain.StaticDetails;

namespace DigitalShoes.Domain.Entities
{
    public class Review : BaseEntity
    {        
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string ReviewText { get; set; }
        public Rating Rating { get; set; }
        public int ShoeId { get; set; }
        public Shoe Shoe { get; set;}
    }
}
