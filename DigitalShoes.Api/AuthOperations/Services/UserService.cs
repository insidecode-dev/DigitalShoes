using DigitalShoes.Api.AuthOperations.Repositories;
using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.AuthDTOs;
using System.Net;

namespace DigitalShoes.Api.AuthOperations.Services
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ApiResponse _apiResponse;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _apiResponse = new();
        }

        public async Task<ApiResponse> LogIn(LogInRequestDto logInRequestDTO)
        {
            var logInResponseDTO = await _userRepository.LogIn(logInRequestDTO);

            if (logInResponseDTO.LocalUser == null || string.IsNullOrEmpty(logInResponseDTO.Token))
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add(logInResponseDTO.ErrorMessage);
                _apiResponse.IsSuccess = false;
                return _apiResponse;
            }

            _apiResponse.IsSuccess = true;
            _apiResponse.Result = logInResponseDTO;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return _apiResponse;
        }

        public async Task<ApiResponse> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            var registrationResponseDTO = await _userRepository.Register(registrationRequestDTO);
            if (registrationResponseDTO.RegisteredUser == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add(registrationResponseDTO.ErrorMessage);
                return _apiResponse;
            }
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Result = registrationResponseDTO.RegisteredUser;
            return _apiResponse;
        }

        public async Task<ApiResponse> AddMyNewRole(MyNewRoleRequestDTO myNewRoleRequestDTO)
        {
            var myNewRoleResponseDTO = await _userRepository.AddMyNewRole(myNewRoleRequestDTO);
            if (!myNewRoleResponseDTO.Succeeded)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add(myNewRoleResponseDTO.Message);
                return _apiResponse;
            }
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Result = myNewRoleResponseDTO;
            return _apiResponse;
        }

        public async Task<ApiResponse> CreateNewRole(NewRoleRequestDTO newRoleRequestDTO)
        {
            var newRoleResponseDTO = await _userRepository.CreateNewRole(newRoleRequestDTO);            
            if (!newRoleResponseDTO.Succeeded)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add(newRoleResponseDTO.Message);
                return _apiResponse;
            }
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Result = newRoleResponseDTO;
            return _apiResponse;
        }
    }
}
