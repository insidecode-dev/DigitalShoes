using AutoMapper;
using DigitalShoes.Dal.Context;
using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.CartItemDTOs;
using DigitalShoes.Domain.Entities;
using DigitalShoes.Domain.FluentValidators;
using DigitalShoes.Service.Abstractions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;


namespace DigitalShoes.Service
{
    public class CartService : ICartService
    {

        private readonly ApplicationDbContext _dbContext;
        //
        private readonly IMapper _mapper;
        protected ApiResponse _apiResponse;
        //
        private readonly UserManager<ApplicationUser> _userManager;

        public CartService(ApplicationDbContext dbContext, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _apiResponse = new();
            _userManager = userManager;
        }

        public async Task<ApiResponse> CreateCartAsync(HttpContext httpContext)
        {
            try
            {
                string username = httpContext
                .User
                .Identities
                .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
                .Value;

                
                var user = await _userManager
                    .Users
                    .Include(u => u.Cart)
                    .FirstOrDefaultAsync(u => u.UserName == username);

                var cart = user.Cart;
                if (cart == null)
                {
                    _dbContext.Carts.AddAsync(new Cart { ApplicationUserId = user.Id });
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    _apiResponse.IsSuccess = true;
                    _apiResponse.StatusCode = HttpStatusCode.OK;
                    _apiResponse.Result = $"you already have card with {cart.Id} id and you have {cart.ItemsCount} items in your cart";
                    return _apiResponse;
                }


                if ((await _dbContext.Carts.Where(x => x.ApplicationUserId == user.Id).FirstOrDefaultAsync()) is not null)
                {
                    _apiResponse.IsSuccess = true;
                    _apiResponse.StatusCode = HttpStatusCode.Created;
                    return _apiResponse;
                }

                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("cart was not created");
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
            catch (Exception ex)
            {
                _apiResponse.ErrorMessages.Add(ex.Message.ToString());
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
        }

        public async Task<ApiResponse> AddToCartAsync(List<CartItemCreateDTO> cartItemCreateDTO, HttpContext httpContext)
        {
            try
            {

                bool hasDuplicateItems = cartItemCreateDTO.GroupBy(n => n.ShoeId).Any(g => g.Count() > 1);
                if (hasDuplicateItems)
                {
                    _apiResponse.ErrorMessages.Add($"you have added same shoes twice or more, you can increase one's count");
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                // validation
                foreach (var item in cartItemCreateDTO)
                {
                    ValidationResult cartItemCreateDTOValidationResult = new CartItemCreateDTOValidator().Validate(item);
                    if (!cartItemCreateDTOValidationResult.IsValid)
                    {
                        _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                        _apiResponse.IsSuccess = false;
                        foreach (var error in cartItemCreateDTOValidationResult.Errors)
                        {
                            _apiResponse.ErrorMessages.Add(error.ErrorMessage);
                        }
                        _apiResponse.Result = cartItemCreateDTOValidationResult;
                        return _apiResponse;
                    }

                    // checking if shoe with provided shoeid exists in database
                    var existingShoe = await _dbContext.Shoes.Where(x => x.Id == item.ShoeId).AsNoTracking().FirstOrDefaultAsync();
                    if (existingShoe is null)
                    {
                        _apiResponse.ErrorMessages.Add($"shoe with {item.ShoeId} id does not exist");
                        _apiResponse.IsSuccess = false;
                        _apiResponse.StatusCode = HttpStatusCode.NotFound;
                        return _apiResponse;
                    }

                    // chechking if requested count is passing existing count
                    if (existingShoe.Count<item.ItemsCount)
                    {
                        _apiResponse.ErrorMessages.Add($"there is not enough shoe with {item.ShoeId} id for your request");
                        _apiResponse.IsSuccess = false;
                        _apiResponse.StatusCode = HttpStatusCode.NotFound;
                        return _apiResponse;
                    }

                }

                string username = httpContext
                .User
                .Identities
                .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
                .Value;

                var user = await _userManager
                    .Users
                    .Include(u => u.Cart)
                    .ThenInclude(ci=>ci.CartItems)
                    .FirstOrDefaultAsync(u => u.UserName == username);

                // checking if user has cart
                var cart = user.Cart;
                if (cart is null)
                {
                    _apiResponse.ErrorMessages.Add("you don't have cart");
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                // checking if user has added same item to his cart before 
                foreach (var item in cartItemCreateDTO)
                {
                    var existingCartItem =  cart.CartItems.Where(x => x.ShoeId == item.ShoeId).FirstOrDefault();
                    if (existingCartItem is not null)
                    {
                        _apiResponse.ErrorMessages.Add($"you have shoe with {item.ShoeId} id in cart, you can increase count");
                        _apiResponse.IsSuccess = false;
                        _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                        return _apiResponse;
                    }
                }

                foreach (var item in cartItemCreateDTO)
                {
                    var cartItem = _mapper.Map<CartItem>(item);
                    cartItem.CartId = cart.Id;

                    // getting price of shoe 
                    decimal priceOfOneShoe = await _dbContext.Shoes.Where(x => x.Id == item.ShoeId).Select(x => x.Price).FirstOrDefaultAsync();

                    cartItem.Price = priceOfOneShoe*item.ItemsCount;

                    await _dbContext.CartItems.AddAsync(cartItem);
                    await _dbContext.SaveChangesAsync();

                    if (await _dbContext.CartItems.Where(x=>x.Id==cartItem.Id).FirstOrDefaultAsync() is null)
                    {
                        _apiResponse.ErrorMessages.Add($"shoe with {item.ShoeId} id could not be added to cart");
                        _apiResponse.IsSuccess = false;
                        _apiResponse.StatusCode= HttpStatusCode.InternalServerError;
                        return _apiResponse;
                    }
                }

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                return _apiResponse;
                
            }
            catch (Exception ex)
            {
                _apiResponse.ErrorMessages.Add(ex.Message.ToString());
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
        }

        public async Task<ApiResponse> MyCartItemsAsync(HttpContext httpContext)
        {
            try
            {
                string username = httpContext
                .User
                .Identities
                .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
                .Value;

                
                var user = await _userManager
                    .Users
                    .Include(u => u.Cart)
                    .ThenInclude(ci=>ci.CartItems)
                    .FirstOrDefaultAsync(u => u.UserName == username);

                // if user has cart
                if (user.Cart is null)
                {
                    _apiResponse.ErrorMessages.Add("you don't have cart");
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                if (user.Cart.CartItems.Count==0)
                {
                    _apiResponse.ErrorMessages.Add("you don't have items in your cart");
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode= HttpStatusCode.OK;
                _apiResponse.Result = _mapper.Map<List<CartItemGetDTO>>(user.Cart.CartItems);
                return _apiResponse;
            }
            catch (Exception ex)
            {
                _apiResponse.ErrorMessages.Add(ex.Message.ToString());
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
        }

        public async Task<ApiResponse> UpdateCartItemCountAsync(int? id, CartItemUpdateDTO cartItemUpdateDTO, HttpContext httpContext)
        {
            try
            {
                if (id == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"id is null");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;                    
                    return _apiResponse;
                }

                // validation
                ValidationResult cartItemUpdateDTOValidationResult = new CartItemUpdateDTOValidator().Validate(cartItemUpdateDTO);
                if (!cartItemUpdateDTOValidationResult.IsValid)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    foreach (var error in cartItemUpdateDTOValidationResult.Errors)
                    {
                        _apiResponse.ErrorMessages.Add(error.ErrorMessage);
                    }
                    _apiResponse.Result = cartItemUpdateDTOValidationResult;
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
                    .Include(u => u.Cart)
                    .ThenInclude(ci => ci.CartItems)
                    .ThenInclude(s=>s.Shoe)
                    .FirstOrDefaultAsync(u => u.UserName == username);

                // checking if cart item with provided id exists
                var cartItem = user.Cart.CartItems.Where(x => x.Id == id).FirstOrDefault();
                if (cartItem == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"you don't have cart item with {id} id");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return _apiResponse;
                }

                // chechking if requested count is passing existing count
                if (cartItem.Shoe.Count < cartItemUpdateDTO.ItemsCount)
                {
                    _apiResponse.ErrorMessages.Add($"there is not enough shoe with {cartItem.Shoe.Id} id for your request");
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                // mapping and updating
                _mapper.Map(cartItemUpdateDTO, cartItem);
                _dbContext.CartItems.Update(cartItem);
                await _dbContext.SaveChangesAsync();

                // checking if operation successful
                if (cartItem.ItemsCount==cartItemUpdateDTO.ItemsCount)
                {
                    _apiResponse.IsSuccess = true;
                    _apiResponse.StatusCode = HttpStatusCode.NoContent;
                    return _apiResponse;
                }

                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add($"count of cartItem with {cartItem.Id} id was not updated");
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
            catch (Exception ex)
            {
                _apiResponse.ErrorMessages.Add(ex.Message.ToString());
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
        }
    }
}
