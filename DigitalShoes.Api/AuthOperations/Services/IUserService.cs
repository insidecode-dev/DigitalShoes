using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.AuthDTOs;

namespace DigitalShoes.Api.AuthOperations.Services
{
    public interface IUserService
    {
        Task<ApiResponse> LogIn(LogInRequestDto logInRequestDTO);
        Task<ApiResponse> Register(RegistrationRequestDTO registrationRequestDTO);
        Task<ApiResponse> AddMyNewRole(MyNewRoleRequestDTO myNewRoleDTO);
        Task<ApiResponse> CreateNewRole(NewRoleRequestDTO newRoleRequestDTO);
    }
}
