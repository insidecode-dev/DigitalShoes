namespace DigitalShoes.Domain.DTOs
{
    public class LogInResponseDTO
    {
        public UserDTO? LocalUser { get; set; }    
        public string? Token { get; set; }      
        public string ErrorMessage { get; set; }
    }
}
