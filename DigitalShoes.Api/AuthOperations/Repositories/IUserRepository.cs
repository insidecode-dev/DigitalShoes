using DigitalShoes.Domain.DTOs.AuthDTOs;

namespace DigitalShoes.Api.AuthOperations.Repositories
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<LogInResponseDTO> LogIn(LogInRequestDto logInRequestDTO);
        Task<RegistrationResponseDTO> Register(RegistrationRequestDTO registrationRequestDTO);
        Task<MyNewRoleResponseDTO> AddMyNewRole(MyNewRoleRequestDTO myNewRoleDTO);
        Task<NewRoleResponseDTO> CreateNewRole(NewRoleRequestDTO newRoleRequestDTO);
    }
}
