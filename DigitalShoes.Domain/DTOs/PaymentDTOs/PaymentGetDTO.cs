using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.DTOs.PaymentDTOs
{
    public class PaymentGetDTO
    {
        public int PaymentId { get; set; }
        public decimal TotalPrice { get; set; }
        public string OrderAdress { get; set; }
        public List<PaymentObjectGetDTO> PaymentObjectsGetDTO { get; set; }
    }
}
