using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.Entities
{
    public class Payment : BaseEntity // ignore ModifiedDate date 
    {
        public int ApplicationUserId { get; set; }    
        public ApplicationUser ApplicationUser { get; set; }        
        public List<PaymentObject> PaymentObjects { get; set; }        
        public decimal TotalPrice { get; set; }
        public string OrderAdress { get; set; }
    }
}