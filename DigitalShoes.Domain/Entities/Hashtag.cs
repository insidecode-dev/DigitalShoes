using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.Entities
{
    public class Hashtag : BaseEntity 
    {        
        public string Text { get; set; }
        public List<ShoeHashtag> ShoeHashtags { get; set; }
    }
}
