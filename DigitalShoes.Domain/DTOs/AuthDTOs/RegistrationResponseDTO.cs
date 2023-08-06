using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.DTOs.AuthDTOs
{
    public class RegistrationResponseDTO
    {
        public UserDTO RegisteredUser { get; set; }
        public string ErrorMessage { get; set; }
    }
}
