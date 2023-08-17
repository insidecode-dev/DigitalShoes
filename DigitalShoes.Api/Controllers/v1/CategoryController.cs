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

        // move to admin
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCategoryAsync([FromForm] CategoryCreateDTO categoryCreateDTO)
        {
            var category = await _categoryService.CreateAsync(categoryCreateDTO);
            //return StatusCode((int)category.StatusCode, category);
            if (category.IsSuccess)
            {
                return CreatedAtRoute("GetCategoryById", routeValues: new { id = (category.Result as CategoryDTO).Id }, value: category);
            }
            return StatusCode((int)category.StatusCode, category);
        }

        [Authorize]
        [HttpGet("{id:int}", Name = "GetCategoryById")]
        public async Task<ActionResult<ApiResponse>> GetCategoryByIdAsync([FromRoute] int? id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return StatusCode((int)category.StatusCode, category);
        }

        [Authorize]
        [HttpDelete("{id:int}", Name = "DeleteCategoryById")]
        public async Task<ActionResult<ApiResponse>> DeleteCategoryByIdAsync([FromRoute] int? id)
        {
            var category = await _categoryService.DeleteCategoryByIdAsync(id);
            return StatusCode((int)category.StatusCode);
        }
    }
}