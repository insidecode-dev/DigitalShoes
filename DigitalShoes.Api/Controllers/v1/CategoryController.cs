using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.CategoryDTOs;
using DigitalShoes.Service.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalShoes.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]        
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddCategoryAsync([FromForm] CategoryCreateDTO categoryCreateDTO)
        {
            var category = await _categoryService.CreateAsync(categoryCreateDTO);            
            if (category.IsSuccess)
            {
                return CreatedAtRoute("GetCategoryById", routeValues: new { id = (category.Result as CategoryDTO).Id }, value: category);
            }
            return StatusCode((int)category.StatusCode, category);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{id:int}", Name = "GetCategoryById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]        
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> GetCategoryByIdAsync([FromRoute] int? id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return StatusCode((int)category.StatusCode, category);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}", Name = "DeleteCategoryById")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> DeleteCategoryByIdAsync([FromRoute] int? id)
        {
            var category = await _categoryService.DeleteCategoryByIdAsync(id);
            return StatusCode((int)category.StatusCode);
        }
    }
}