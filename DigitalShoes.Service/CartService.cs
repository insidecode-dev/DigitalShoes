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
using Microsoft.EntityFrameworkCore.Storage;
using System.Net;
using System.Runtime.InteropServices;
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
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();
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
                        await _dbContextTransaction.RollbackAsync();
                        _apiResponse.ErrorMessages.Add($"shoe with {item.ShoeId} id could not be added to cart");
                        _apiResponse.IsSuccess = false;
                        _apiResponse.StatusCode= HttpStatusCode.InternalServerError;
                        return _apiResponse;
                    }
                }

                //updating count and totalprice of cart 
                var prc = await _dbContext.CartItems.Where(x => x.CartId == cart.Id).Select(x => x.Price).SumAsync();
                var cnt = await _dbContext.CartItems.Where(x => x.CartId == cart.Id).Select(x=>x.ItemsCount).SumAsync();
                cart.ItemsCount = cnt;
                cart.TotalPrice = prc;
                
                _dbContext.Carts.Update(cart);
                await _dbContext.SaveChangesAsync();

                // checking if totalprice or count updated
                
                if (cart.ItemsCount != cnt || cart.TotalPrice!=prc)
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
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                return _apiResponse;
                
            }
            catch (Exception ex)
            {
                await _dbContextTransaction.RollbackAsync();
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
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();
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

                // 
                var existingShoe = user.Cart.CartItems.Where(x => x.Id == id).Select(x=>x.Shoe).FirstOrDefault();
                

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
                if (cartItem.Shoe.Count < cartItemUpdateDTO.ItemsCount || cartItem.Shoe.Count==0)
                {
                    _apiResponse.ErrorMessages.Add($"there is not enough shoe with {cartItem.Shoe.Id} id for your request");
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                // mapping and updating cartitem
                _mapper.Map(cartItemUpdateDTO, cartItem);
                cartItem.Price = existingShoe.Price * cartItemUpdateDTO.ItemsCount;
                _dbContext.CartItems.Update(cartItem);
                await _dbContext.SaveChangesAsync();

                // checking if cartitem count updated
                if (await _dbContext.CartItems.Where(x=>x.Id==cartItem.Id).Select(x=>x.ItemsCount).FirstOrDefaultAsync()!=cartItemUpdateDTO.ItemsCount)
                {
                    await _dbContextTransaction.RollbackAsync();
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                    _apiResponse.ErrorMessages.Add("operation is not successful");
                    return _apiResponse;
                }

                //
                var cart = await _dbContext.CartItems.Include(x=>x.Cart).Where(x=>x.Id==id).Select(x=>x.Cart).FirstOrDefaultAsync();
                var prc = cart.CartItems.Select(x => x.Price).Sum(); //_dbContext.CartItems.Where(x => x.CartId == cart.Id).Select(x=>x.Price).Sum();
                var cnt = cart.CartItems.Select(x => x.ItemsCount).Sum();
                cart.TotalPrice = prc;
                cart.ItemsCount = cnt;
                _dbContext.Carts.Update(cart);
                await _dbContext.SaveChangesAsync();

                // checking if totalprice and count of cart updated                
                if (await _dbContext.Carts.Where(x=>x.Id==cart.Id).Select(x=>x.TotalPrice).FirstOrDefaultAsync() != prc ||
                    await _dbContext.Carts.Where(x => x.Id == cart.Id).Select(x => x.ItemsCount).FirstOrDefaultAsync() != cnt)
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
            catch (Exception ex)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.ErrorMessages.Add(ex.Message.ToString());
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
        }
        public async Task<ApiResponse> MyCartItemAsync(int? id, HttpContext httpContext)
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
                    .FirstOrDefaultAsync(u => u.UserName == username);

                // if user has cart
                var cart = user.Cart;
                if (cart is null)
                {
                    _apiResponse.ErrorMessages.Add("you don't have cart");
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                // if user have item in his cart
                if (cart.CartItems.Count == 0)
                {
                    _apiResponse.ErrorMessages.Add("you don't have items in your cart");
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                var cartItem = cart.CartItems.Where(x=>x.Id==id).FirstOrDefault();
                if (cartItem is null)
                {
                    _apiResponse.ErrorMessages.Add($"you don't have item with {id} id in your cart");
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return _apiResponse;
                }
                

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.Result = _mapper.Map<CartItemGetDTO>(cartItem);
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

        public async Task<ApiResponse> RemoveCartItemAsync(int? id, HttpContext httpContext)
        {
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (id == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"id is null");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
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

                // removing cartitem
                _dbContext.CartItems.Remove(cartItem);
                await _dbContext.SaveChangesAsync();

                // chechking if cartitem with the provided id exists in cart 
                if (await _dbContext.CartItems.Where(x => x.Id==id).FirstOrDefaultAsync() != null)
                {
                    await _dbContextTransaction.RollbackAsync();
                    _apiResponse.ErrorMessages.Add($"your cartitem with {id} id was not removed");
                    _apiResponse.IsSuccess =false;
                    _apiResponse.StatusCode=HttpStatusCode.InternalServerError;
                    return _apiResponse;
                }


                //var cart =  await _dbContext.CartItems.Where(x => x.Id == id).Include(x => x.Cart).Select(x=>x.Cart).FirstOrDefaultAsync();
                var cart = user.Cart;// await _dbContext.CartItems.Include(x => x.Cart).Where(x => x.Id == id).Select(x => x.Cart).FirstOrDefaultAsync();

                //var seller = await _dbContext
                //    .Shoes
                //    .Include(x => x.ApplicationUser)
                //    .Where(x => x.Id == item.ShoeId)
                //    .Select(x => x.ApplicationUser)
                //    .FirstOrDefaultAsync();

                // updating price
                var prc = await _dbContext.CartItems.Where(x => x.CartId == cart.Id).Select(x => x.Price).SumAsync();
                cart.TotalPrice = prc;

                // updating count 
                var cnt = await _dbContext.CartItems.Where(x => x.CartId == cart.Id).Select(x => x.ItemsCount).SumAsync();
                cart.ItemsCount = cnt;

                // updating complete
                _dbContext.Carts.Update(cart);
                await _dbContext.SaveChangesAsync();

                // checking if totalprice or count updated                
                if (await _dbContext.Carts.Where(x => x.Id == cart.Id && x.TotalPrice==prc && x.ItemsCount==cnt).FirstOrDefaultAsync() is null)
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
                _apiResponse.IsSuccess=true;
                _apiResponse.StatusCode = HttpStatusCode.NoContent; 
                return _apiResponse;
            }
            catch (Exception ex)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.ErrorMessages.Add(ex.Message.ToString());
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
        }
    }
}
