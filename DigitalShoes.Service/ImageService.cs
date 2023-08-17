using AutoMapper;
using DigitalShoes.Dal.Context;
using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.ImageDTOs;
using DigitalShoes.Domain.DTOs.ShoeDTOs;
using DigitalShoes.Domain.Entities;
using DigitalShoes.Domain.FluentValidators;
using DigitalShoes.Service.Abstractions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace DigitalShoes.Service
{
    public class ImageService : IMageService
    {   
        private readonly ApplicationDbContext _dbContext;
        //
        private readonly IMapper _mapper;
        protected ApiResponse _apiResponse;
        private readonly IWebHostEnvironment _webHostEnvironment;
        //
        private readonly UserManager<ApplicationUser> _userManager;

        public ImageService(IMapper mapper, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            _mapper = mapper;
            _apiResponse = new();
            _webHostEnvironment = webHostEnvironment;            
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<ApiResponse> CreateAsync(ImageCreateDTO imageCreateDTO, HttpContext httpContext)
        {
            try
            {
                ValidationResult imageValidationResult = new ImageCreateDTOValidator().Validate(imageCreateDTO);
                if (!imageValidationResult.IsValid)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    foreach (var error in imageValidationResult.Errors)
                    {
                        _apiResponse.ErrorMessages.Add(error.ErrorMessage);
                    }
                    _apiResponse.Result = imageCreateDTO;
                    return _apiResponse;
                }

                var username = httpContext
                .User
                .Identities
                .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
                .Value;

                var user = await _userManager
                .Users
                .Include(x => x.Shoes)
                .FirstOrDefaultAsync(u => u.UserName == username);



                var existingShoe = user.Shoes.Where(sh => sh.Id == imageCreateDTO.ShoeId).FirstOrDefault();
                if (existingShoe == null)
                {
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"You don't have any product with {imageCreateDTO.ShoeId} id");
                    return _apiResponse;
                }


                // image (validation inside)
                if (!Directory.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "ProductImage", username + "Products")))
                {
                    // Folder does not exist, create a new folder with the specified name
                    Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, "ProductImage", username + "Products"));
                }

                foreach (var item in imageCreateDTO.Image)
                {
                    //
                    string fileName = Guid.NewGuid().ToString().ToString() + Path.GetExtension(item.FileName);

                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "ProductImage", username + "Products", fileName).Replace("\\", "/");


                    // adding file to specified folder 
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await item.CopyToAsync(fileStream);
                    }

                    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host.Value}";

                    var image = new Image
                    {
                        // generating and initializing url for image on web 
                        ImageUrl = Path.Combine(baseUrl, "ProductImage", username + "Products", fileName).Replace("\\", "/"),

                        // initializing localpath to image file inside project
                        ImageLocalPath = filePath,
                        ShoeId = existingShoe.Id
                    };

                    //                    
                    await _dbContext.Images.AddAsync(image);
                    await _dbContext.SaveChangesAsync();

                    var ifCreated = await _dbContext.Images.Where(img => img.ImageLocalPath.Contains(fileName)).FirstOrDefaultAsync();
                    if (ifCreated == null)
                    {
                        _apiResponse.IsSuccess = false;
                        _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                        _apiResponse.ErrorMessages.Add($"error when adding {fileName} image");
                        return _apiResponse;
                    }
                }

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                _apiResponse.Result = _mapper.Map<List<ImageDTO>>(await _dbContext.Images.Where(img => img.ShoeId == existingShoe.Id).ToListAsync());
                return _apiResponse;
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add(ex.Message);
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.Result = ex;
                return _apiResponse;
            }
        }

        public async Task<ApiResponse> DeleteAsync(ImageDeleteDTO imageDeleteDTO, HttpContext httpContext)
        {
            try
            {
                ValidationResult imageDeleteDTOValidationResult = new ImageDeleteDTOValidator().Validate(imageDeleteDTO);
                if (!imageDeleteDTOValidationResult.IsValid)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    foreach (var error in imageDeleteDTOValidationResult.Errors)
                    {
                        _apiResponse.ErrorMessages.Add(error.ErrorMessage);
                    }
                    _apiResponse.Result = imageDeleteDTO;
                    return _apiResponse;
                }

                var username = httpContext
                .User
                .Identities
                .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
                .Value;


                var user = await _userManager
                    .Users
                    .Include(u => u.Shoes)
                    .ThenInclude(img => img.Images)
                    .FirstOrDefaultAsync(u => u.UserName == username);


                var existingShoe = user.Shoes.Where(x => x.Id == imageDeleteDTO.ShoeId).FirstOrDefault();                
                if (existingShoe == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"You don't have any product with {imageDeleteDTO.ShoeId} id");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;                    
                    return _apiResponse;
                }


                var existingImage = existingShoe.Images.Where(x => x.Id == imageDeleteDTO.ImageId).FirstOrDefault();
                if (existingImage == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"You don't have a image with {imageDeleteDTO.ImageId} id of any shoe that id is  {imageDeleteDTO.ShoeId}");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;                    
                    return _apiResponse;
                }

                if (!string.IsNullOrEmpty(existingImage.ImageLocalPath))
                {
                    FileInfo file = new FileInfo(existingImage.ImageLocalPath);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }

                
                _dbContext.Images.Remove(existingImage);
                await _dbContext.SaveChangesAsync();

                var ifDeleted = user.Shoes.Where(x => x.Id == imageDeleteDTO.ShoeId).FirstOrDefault().Images.Where(x => x.Id == imageDeleteDTO.ImageId).FirstOrDefault();
                if (ifDeleted == null)
                {
                    _apiResponse.IsSuccess = true;
                    _apiResponse.StatusCode = HttpStatusCode.NoContent;                  
                    return _apiResponse;
                }

                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages.Add($"your image that id was {imageDeleteDTO.ImageId} of shoe that id is {imageDeleteDTO.ShoeId} was not deleted");
                return _apiResponse;
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add(ex.Message);
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Result = ex;
                return _apiResponse;
            }
        }
    }
}
