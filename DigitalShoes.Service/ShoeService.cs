using DigitalShoes.Dal.Repository.Interfaces;
using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.ProductDTOs;
using DigitalShoes.Service.Abstractions;
using DigitalShoes.Domain.FluentValidators;
using DigitalShoes.Domain.Entities;
using FluentValidation.Results;
using AutoMapper;
using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace DigitalShoes.Service
{
    public class ShoeService:IShoeService
    {
        private readonly IShoeRepository _shoeRepository;
        private readonly IMageRepository _imageRepository;
        private readonly IMapper _mapper;
        protected ApiResponse _apiResponse;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ShoeService(IMapper mapper, IShoeRepository shoeRepository, IMageRepository imageRepository, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _shoeRepository = shoeRepository;
            _apiResponse = new();
            _imageRepository = imageRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ApiResponse> Create(ShoeCreateDTO shoeCreateDTO, string username, HttpRequest httpRequest)
        {
            try
            {
                // shoe
                var shoe = _mapper.Map<Shoe>(shoeCreateDTO);
                ValidationResult shoeValidationResult = new ShoeValidator().Validate(shoe);
                if (!shoeValidationResult.IsValid)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    foreach (var error in shoeValidationResult.Errors)
                    {
                        _apiResponse.ErrorMessages.Add(error.ErrorMessage);
                    }
                    return _apiResponse;
                }
                await _shoeRepository.CreateAsync(shoe);

                // image (no validation)
                if (!Directory.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "ProductImage", username)))
                {
                    // Folder does not exist, create a new folder with the specified name
                    Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, "ProductImage", username));                    
                }

                foreach (var item in shoeCreateDTO._Images)
                {
                    item.ShoeId = shoe.Id;
                    if (item.Image != null)
                    {
                        
                        string fileName = new Guid().ToString() + Path.GetExtension(item.Image.FileName);
                        
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "ProductImage", username, fileName);

                        // creating path to file, includes path to project
                        var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                        

                        // adding file to specified folder 
                        using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                        {
                            await item.Image.CopyToAsync(fileStream);
                        }


                        // generating section url for image on web 

                        // HttpContext.Request.Scheme => returns https of https://example.com/somepage
                        // HttpContext.Request.Host.Value => returns localhost:44329 of https://localhost:44329
                        // HttpContext.Request.PathBase.Value => returns /myapp of https://example.com/myapp/home/index

                        var baseUrl = $"{httpRequest.Scheme}://{httpRequest.Host.Value}";

                        // generating and initializing url for image on web 
                        item.ImageUrl = baseUrl + "/ProductImage/" + fileName;

                        // initializing localpath to image file inside project
                        item.ImageLocalPath = filePath;
                    }
                    else
                    {
                        item.ImageUrl = "htttps://placehold.co/600x400";
                    }

                    var image = _mapper.Map<Image>(item);

                    await _imageRepository.CreateAsync(image);
                }

                
                //var existingShoe = _shoeRepository.GetAsync(sh => sh.Brand == shoeCreateDTO.Brand && sh.Model == shoeCreateDTO.Model);
                //if (existingShoe != null)
                //{
                //    _apiResponse.IsSuccess = false;
                //    _apiResponse.ErrorMessages.Add("");

                //    return new ApiResponse()
                //}


                return _apiResponse;
            }
            catch (Exception)
            {

                throw;
            }            
        }
    }
}
