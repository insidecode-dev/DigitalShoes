using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.DTOs.SearchDTOs
{
    public class SearchByHashtagRequestDTO
    {
        public int PageSize { get; set; } = 3;
        public int PageNumber { get; set; } = 1;
        public string Text { get; set; }
    }
}
