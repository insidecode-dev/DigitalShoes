using DigitalShoes.Domain.DTOs;

namespace DigitalShoes.Api.AuthOperations.Repositories
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<LogInResponseDTO> LogIn(LogInRequestDto logInRequestDTO);
        Task<RegistrationResponseDTO> Register(RegistrationRequestDTO registrationRequestDTO);
    }
}
