using AutoMapper;
using DigitalShoes.Dal.Repository.Interfaces;
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
using System;
using System.Collections.Generic;

using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Service
{
    public class ImageService : IMageService
    {
        private readonly IShoeRepository _shoeRepository;
        private readonly IMageRepository _imageRepository;
        //
        private readonly IMapper _mapper;
        protected ApiResponse _apiResponse;
        private readonly IWebHostEnvironment _webHostEnvironment;
        //
        private readonly UserManager<ApplicationUser> _userManager;

        public ImageService(IMapper mapper, IWebHostEnvironment webHostEnvironment, IShoeRepository shoeRepository, IMageRepository imageRepository, UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _apiResponse = new();
            _webHostEnvironment = webHostEnvironment;
            _shoeRepository = shoeRepository;
            _imageRepository = imageRepository;
            _userManager = userManager;
        }

        public async Task<ApiResponse> CreateAsync(ImageCreateDTO imageCreateDTO, string username, HttpRequest httpRequest)
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

                // checking shoe 
                var existingShoe = await _shoeRepository.GetAsync(sh => sh.Id == imageCreateDTO.ShoeId);
                if (existingShoe == null)
                {
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add("shoe not found");
                    _apiResponse.Result = imageCreateDTO;
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
                    
                    var baseUrl = $"{httpRequest.Scheme}://{httpRequest.Host.Value}";

                    var image = new Image
                    {
                        // generating and initializing url for image on web 
                        ImageUrl = Path.Combine(baseUrl, "ProductImage", username + "Products", fileName).Replace("\\", "/"),

                        // initializing localpath to image file inside project
                        ImageLocalPath = filePath,
                        ShoeId = existingShoe.Id
                    };

                    //
                    await _imageRepository.CreateAsync(image);
                }

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                _apiResponse.Result = _mapper.Map<List<ImageDTO>>(await _imageRepository.GetAllAsync(img => img.ShoeId == existingShoe.Id));
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

        public async Task<ApiResponse> DeleteAsync(ImageDeleteDTO imageDeleteDTO, string username)
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

                var user = await _userManager
                    .Users                    
                    .Include(u => u.Shoes)
                    .ThenInclude(img=>img.Images)
                    .FirstOrDefaultAsync(u => u.UserName == username);


                var existingImage = user.Shoes.Where(x => x.Id == imageDeleteDTO.ShoeId).FirstOrDefault().Images.Where(x=>x.Id==imageDeleteDTO.ImageId).FirstOrDefault();

                if (existingImage == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"You don't have a image with {imageDeleteDTO.ImageId} id of any shoe that id is  {imageDeleteDTO.ShoeId}");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.Result= imageDeleteDTO;
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

                await _imageRepository.RemoveAsync(existingImage);
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                _apiResponse.Result= $"your image that id was {imageDeleteDTO.ImageId} of shoe that id is {imageDeleteDTO.ShoeId} was deleted";
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
    }
}
