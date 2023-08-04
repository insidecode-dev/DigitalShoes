using AutoMapper;
using DigitalShoes.Dal.Context;
using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;


namespace DigitalShoes.Api.AuthOperations.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        private string? secretKey;

        // after identity 
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        public UserRepository(ApplicationDbContext dbContext, IConfiguration configuration, UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole<int>> roleManager)
        {
            _dbContext = dbContext;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public bool IsUniqueUser(string username)
        {
            var user = _dbContext
                       .ApplicationUsers
                       .FirstOrDefault(x => x.UserName == username);

            if (user == null) { return true; }
            return false;
        }

        public async Task<LogInResponseDTO> LogIn(LogInRequestDto logInRequestDTO)
        {
            ApplicationUser? user = await _dbContext
                             .ApplicationUsers
                             .FirstOrDefaultAsync(x => x.UserName.ToLower() == logInRequestDTO.UserName.ToLower());

            // checking if the password of user found by username is same with password in requestlogin
            var isValidPassword = await _userManager.CheckPasswordAsync(user, logInRequestDTO.Password);

            // validating all two checks above 
            if (user is null || !isValidPassword)
            {
                return new LogInResponseDTO()
                {
                    LocalUser = null,
                    Token = "",
                    ErrorMessage = "username or password is incorrect"
                };
            }

            // we use user's role when generating token inside tokenDescription, that's why we retrieve user's role
            var roles = await _userManager.GetRolesAsync(user);
            var logInRole = roles.FirstOrDefault(r => r == logInRequestDTO.Role);
            if (logInRole == null)
            {
                return new LogInResponseDTO()
                {
                    LocalUser = null,
                    Token = "",
                    ErrorMessage = $"you don't have {logInRequestDTO.Role} account"
                };
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            // this line converts secret key to bytes and we'll have that as byte array in the variable => key
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.Name.ToString()),
                        new Claim(ClaimTypes.Email, user.Email.ToString()),
                        new Claim(ClaimTypes.Role, logInRole)
                    }),

                Expires = DateTime.UtcNow.AddDays(7),

                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return new LogInResponseDTO()
            {
                LocalUser = _mapper.Map<UserDTO>(user),
                Token = tokenHandler.WriteToken(token)
            };
        }

        public async Task<RegistrationResponseDTO> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            try
            {
                var isUserNameUnique = IsUniqueUser(registrationRequestDTO.UserName);
                if (!isUserNameUnique)
                {
                    return new RegistrationResponseDTO
                    {
                        RegisteredUser = null,
                        ErrorMessage = "username already exists"
                    };
                }

                ApplicationUser localUser = new()
                {
                    UserName = registrationRequestDTO.UserName,
                    Email = registrationRequestDTO.Email,
                    NormalizedEmail = registrationRequestDTO.Email.ToUpper(),
                    Name = registrationRequestDTO.Name
                };

                // validating role 
                string? validateRole = registrationRequestDTO.Role.FirstOrDefault(x => !_roleManager.RoleExistsAsync(x).GetAwaiter().GetResult() || x.ToLower() == "admin")?.ToLower();
                if (validateRole == "admin")
                {
                    return new RegistrationResponseDTO { RegisteredUser = null, ErrorMessage = $"{validateRole} role is not allowed" };
                }
                else if (validateRole != null)
                {
                    return new RegistrationResponseDTO { RegisteredUser = null, ErrorMessage = $"{validateRole} role does not exist" };
                }

                var result = await _userManager.CreateAsync(user: localUser, password: registrationRequestDTO.Password);

                if (result.Succeeded)
                {                       
                    foreach (var role in registrationRequestDTO.Role)
                    {
                        await _userManager.AddToRoleAsync(user: localUser, role: role);
                    }                    
                    var createdUser = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.UserName == registrationRequestDTO.UserName);
                    return new RegistrationResponseDTO { RegisteredUser = _mapper.Map<UserDTO>(createdUser) };
                }
                else
                {
                    return new RegistrationResponseDTO { RegisteredUser = null, ErrorMessage = result.Errors.FirstOrDefault().ToString() };
                }
            }
            catch (Exception ex)
            {
                return new RegistrationResponseDTO { RegisteredUser = null, ErrorMessage = ex.Message.ToString() };
            }

         }

        public async Task<MyNewRoleResponseDTO> AddMyNewRole(MyNewRoleRequestDTO myNewRoleDTO)
        {
            try
            {
                ApplicationUser? user = await _dbContext
                             .ApplicationUsers
                             .FirstOrDefaultAsync(x => x.UserName.ToLower() == myNewRoleDTO.UserName.ToLower());

                var roles = await _userManager.GetRolesAsync(user);
                var newRole = roles.FirstOrDefault(r => r == myNewRoleDTO.RoleName);

                if (newRole != null)
                {
                    return new MyNewRoleResponseDTO() { Message = $"you have {newRole} account", Succeeded = false };
                }

                await _userManager.AddToRoleAsync(user: user, role: myNewRoleDTO.RoleName);
                return new MyNewRoleResponseDTO() { Message = $"Your {myNewRoleDTO.RoleName} account created successfully !!", Succeeded = true };

            }
            catch (Exception ex)
            {
                return new MyNewRoleResponseDTO() { Message = ex.Message.ToString(), Succeeded = false };
            }

        }

        public async Task<NewRoleResponseDTO> CreateNewRole(NewRoleRequestDTO newRoleRequestDTO)
        {
            try
            {
                if (!_roleManager.RoleExistsAsync(newRoleRequestDTO.RoleName).GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole<int>(newRoleRequestDTO.RoleName));
                    return new NewRoleResponseDTO
                    {
                        Message = $"{newRoleRequestDTO.RoleName} role created successfully",
                        Succeeded = true
                    };
                }
                else return new NewRoleResponseDTO
                {
                    Message = $"{newRoleRequestDTO.RoleName} role role already exists",
                    Succeeded = false
                };
            }
            catch (Exception ex)
            {
                return new NewRoleResponseDTO
                {
                    Message = $"{ex.Message.ToString()} error while creating {newRoleRequestDTO.RoleName} role",
                    Succeeded = false
                };
            }

        }
    }
}
