namespace DigitalShoes.Domain.DTOs.AuthDTOs
{
    public class LogInRequestDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
