using AutoMapper;
using DigitalShoes.Dal.Context;
using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.AuthDTOs;
using DigitalShoes.Domain.DTOs.UserProfileDTOs;
using DigitalShoes.Domain.Entities;
using DigitalShoes.Domain.FluentValidators;
using DigitalShoes.Service.Abstractions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;


namespace DigitalShoes.Service
{
    public class UserProfileService : IUserProfileService
    {
        private readonly ApplicationDbContext _dbContext;
        //
        private readonly IMapper _mapper;
        protected ApiResponse _apiResponse;
        //
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        //
        private string? secretKey;
        public UserProfileService(ApplicationDbContext dbContext, IMapper mapper, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _apiResponse = new();
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public async Task<ApiResponse> IncreaseBalanceAsync(decimal balance, HttpContext httpContext)
        {
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

            string username = httpContext
            .User
            .Identities
            .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
            .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
            .Value;

            var user = await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            // checking if user's balance is higher than requested
            if (user.Balance > balance)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add($"requested balance income is lower than your balance");
                return _apiResponse;
            }

            // updating user's balance
            user.Balance = balance;
            //_userManager.UpdateAsync(user);
            await _dbContext.SaveChangesAsync();
            // checking if user's balance updated
            var updatedBalance = await _userManager
                .Users
                .Where(u => u.UserName == username)
                .Select(x => x.Balance)
                .FirstOrDefaultAsync();
            if (balance != updatedBalance)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages.Add("operation is not successful");
                return _apiResponse;
            }

            // transaction finished
            _dbContextTransaction.Commit();

            // response                 
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            return _apiResponse;
        }

        public async Task<ApiResponse> AddProfileImageAsync(IFormFile image, HttpContext httpContext)
        {
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

            string username = httpContext
            .User
            .Identities
            .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
            .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
            .Value;

            var user = await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user.ProfileImageLocalPath != null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("you already have profile image");
                return _apiResponse;
            }

            if (image is null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("image must be uploaded");
                return _apiResponse;
            }

            if (!image.ContentType.Contains("image"))
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("you must upload just image");
                return _apiResponse;
            }

            //
            string fileName = user.UserName + "Image" + Path.GetExtension(image.FileName);

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "ProfileImage", fileName).Replace("\\", "/");


            // adding file to specified folder 
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host.Value}";

            var imageUrl = Path.Combine(baseUrl, "ProfileImage", fileName).Replace("\\", "/");

            user.ProfileImageLocalPath = filePath;
            user.ProfileImageUrl = imageUrl;
            await _dbContext.SaveChangesAsync();
            // checking if profile image created
            var updatedUser = await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.UserName == username);
            if (updatedUser.ProfileImageLocalPath != filePath || updatedUser.ProfileImageUrl != imageUrl)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages.Add("operation is not successful");

                if (!string.IsNullOrEmpty(filePath))
                {
                    FileInfo file = new FileInfo(filePath);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }

                return _apiResponse;
            }

            // transaction finished
            _dbContextTransaction.Commit();

            // response                 
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.Created;
            _apiResponse.Result = new ProfileImageGetDTO { ProfileImageLocalPath = filePath, ProfileImageUrl = imageUrl };
            return _apiResponse;
        }

        public async Task<ApiResponse> UpdateProfileImageAsync(IFormFile image, HttpContext httpContext)
        {
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

            string username = httpContext
            .User
            .Identities
            .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
            .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
            .Value;

            var user = await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user.ProfileImageLocalPath == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("you don't have profile image");
                return _apiResponse;
            }

            if (image is null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("image must be uploaded");
                return _apiResponse;
            }

            if (!image.ContentType.Contains("image"))
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("you must upload just image");
                return _apiResponse;
            }

            //
            string fileName = user.UserName + "Image" + Path.GetExtension(image.FileName);

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "ProfileImage", fileName).Replace("\\", "/");


            FileInfo _file = new FileInfo(user.ProfileImageLocalPath);
            if (_file.Exists)
            {
                _file.Delete();
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }


            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host.Value}";

            var imageUrl = Path.Combine(baseUrl, "ProfileImage", fileName).Replace("\\", "/");

            user.ProfileImageLocalPath = filePath;
            user.ProfileImageUrl = imageUrl;
            await _dbContext.SaveChangesAsync();
            // checking if profile image created
            var updatedUser = await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.UserName == username);
            if (updatedUser.ProfileImageLocalPath != filePath || updatedUser.ProfileImageUrl != imageUrl)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages.Add("operation is not successful");
                return _apiResponse;
            }

            // transaction finished
            _dbContextTransaction.Commit();

            // response                 
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            return _apiResponse;
        }

        public async Task<ApiResponse> DeleteProfileImageAsync(HttpContext httpContext)
        {
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

            string username = httpContext
            .User
            .Identities
            .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
            .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
            .Value;

            var user = await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user.ProfileImageLocalPath == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("you don't have profile image");
                return _apiResponse;
            }

            var filePath = user.ProfileImageLocalPath;

            user.ProfileImageLocalPath = null;
            user.ProfileImageUrl = null;
            await _dbContext.SaveChangesAsync();

            // checking if profile image deleted
            var updatedUser = await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.UserName == username);
            if (updatedUser.ProfileImageUrl != null || updatedUser.ProfileImageLocalPath != null)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages.Add("operation is not successful");
                return _apiResponse;
            }

            // transaction finished
            _dbContextTransaction.Commit();

            //
            FileInfo _file = new FileInfo(filePath);
            if (_file.Exists)
            {
                _file.Delete();
            }

            // response                 
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            return _apiResponse;
        }

        public async Task<ApiResponse> UpdateOrderAdressAsync(string? orderAdress, HttpContext httpContext)
        {
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

            if (orderAdress == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("you must add order adress .");
                return _apiResponse;
            }

            string username = httpContext
            .User
            .Identities
            .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
            .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
            .Value;

            var user = await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            // updating orderadress
            user.OrderAdress = orderAdress;
            await _userManager.UpdateAsync(user);

            // checking if order adress updated
            var updatedUser = await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.UserName == username);
            if (updatedUser.OrderAdress != orderAdress)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages.Add("operation is not successful");
                return _apiResponse;
            }

            // transaction finished
            _dbContextTransaction.Commit();

            // response 
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            return _apiResponse;
        }

        public async Task<ApiResponse> UpdateUserNameAsync(string? username, HttpContext httpContext)
        {
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

            if (username == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("you must add username .");
                return _apiResponse;
            }

            string existingUsername = httpContext
            .User
            .Identities
            .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
            .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
            .Value;

            // checking if this username is used by user 
            if (existingUsername == username)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("you already have this username");
                return _apiResponse;
            }

            var user = await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.UserName == existingUsername);

            // checking if username exists
            var ifUsernameExists = await _userManager.Users.Where(x => x.UserName == username).Select(x => x.UserName).ToListAsync();
            if (ifUsernameExists.Count != 0)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("this username already exists");
                return _apiResponse;
            }

            // updating username
            user.UserName = username;
            await _userManager.UpdateAsync(user);

            // checking if username updated
            var updatedUser = await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.UserName == username);
            if (updatedUser.UserName is null)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages.Add("operation is not successful");
                return _apiResponse;
            }

            // transaction finished
            _dbContextTransaction.Commit();

            // generating new token after updating username
            var tokenHandler = new JwtSecurityTokenHandler();
            var _role = httpContext
            .User
            .Identities
            .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Role))?
            .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?
            .Value;
            // this line converts secret key to bytes and we'll have that as byte array in the variable => key
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.UserName.ToString()),
                        new Claim(ClaimTypes.Email, user.Email.ToString()),
                        new Claim(ClaimTypes.Role, _role)
                    }),

                Audience = "https://localhost:7249/",

                Issuer = "https://localhost:7249/",

                Expires = DateTime.UtcNow.AddMinutes(10),

                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            // response 
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Result = tokenHandler.WriteToken(token);
            return _apiResponse;
        }      


        

        public async Task<ApiResponse> UpdatePasswordAsync(UpdatePasswordDTO updatePasswordDTO, HttpContext httpContext)
        {
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

            ValidationResult updatePasswordDTOValidationResult = new UpdatePasswordDTOValidator().Validate(updatePasswordDTO);
            if (!updatePasswordDTOValidationResult.IsValid)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                foreach (var error in updatePasswordDTOValidationResult.Errors)
                {
                    _apiResponse.ErrorMessages.Add(error.ErrorMessage);
                }
                _apiResponse.Result = updatePasswordDTOValidationResult;
                return _apiResponse;
            }

            string username = httpContext
            .User
            .Identities
            .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
            .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
            .Value;

            var user = await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            // updating and checking if password updated
            var result = await _userManager.ChangePasswordAsync(user, updatePasswordDTO.CurrentPassword, updatePasswordDTO.NewPassword);
            if (!result.Succeeded)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                foreach (var error in result.Errors)
                {
                    _apiResponse.ErrorMessages.Add(error.Description.ToString());
                }
                return _apiResponse;
            }

            // transaction finished
            _dbContextTransaction.Commit();

            // response 
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            return _apiResponse;
        }

        public async Task<ApiResponse> DeleteMyAccountAsync(HttpContext httpContext)
        {
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();


            string username = httpContext
            .User
            .Identities
            .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
            .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
            .Value;

            var user = await _userManager
                .Users
                .Where(u => u.UserName == username)
                .Include(x => x.Shoes)
                .ThenInclude(x => x.Images)
                .FirstOrDefaultAsync();
            string? profileImage = user.ProfileImageLocalPath;
            List<Image> _images = null;
            if (user.Shoes.SelectMany(x => x.Images).ToList() != null || user.Shoes.SelectMany(x => x.Images).ToList().Count != 0)
            {
                _images = user.Shoes.SelectMany(x => x.Images).ToList();
            }

            //    var user = await _applicationDbContext.ApplicationUsers.Where(x => x.Id == 2)
            //.Include(x => x.Shoes)
            //.FirstOrDefaultAsync();

            // removing user
            await _userManager.DeleteAsync(user);

            // checking if user removed
            var removedUser = await _userManager
                .Users
                .Where(u => u.UserName == username)
                .FirstOrDefaultAsync();
            if (removedUser != null)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages.Add("operation is not successful");
                return _apiResponse;
            }

            // transaction finished
            _dbContextTransaction.Commit();

            // removing images of related shoes            
            if (_images != null || _images.Count != 0)
            {
                foreach (var _image in _images)
                {
                    if (!string.IsNullOrEmpty(_image.ImageLocalPath))
                    {
                        FileInfo file = new FileInfo(_image.ImageLocalPath);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }
                }
            }

            // removing profile image
            if (!string.IsNullOrEmpty(profileImage))
            {
                FileInfo file = new FileInfo(profileImage);
                if (file.Exists)
                {
                    file.Delete();
                }
            }


            // response 
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            return _apiResponse;
        }
    }
}
