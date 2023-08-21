using AutoMapper;
using DigitalShoes.Dal.Context;
using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.UserProfileDTOs;
using DigitalShoes.Domain.Entities;
using DigitalShoes.Service.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Net;
using System.Security.Claims;


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
        public UserProfileService(ApplicationDbContext dbContext, IMapper mapper, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _apiResponse = new();
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
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
    }
}
