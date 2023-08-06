namespace DigitalShoes.Domain.DTOs.AuthDTOs
{
    public class RegistrationRequestDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public List<string> Role { get; set; }
    }
}
