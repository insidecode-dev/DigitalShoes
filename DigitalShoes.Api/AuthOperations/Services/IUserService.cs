using DigitalShoes.Domain.DTOs;

namespace DigitalShoes.Api.AuthOperations.Services
{
    public interface IUserService
    {
        Task<ApiResponseDTO> LogIn(LogInRequestDto logInRequestDTO);
        Task<ApiResponseDTO> Register(RegistrationRequestDTO registrationRequestDTO);
        Task<ApiResponseDTO> AddMyNewRole(MyNewRoleRequestDTO myNewRoleDTO);
    }
}
