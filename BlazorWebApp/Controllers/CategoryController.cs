using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MauiMarket.Shared.Models;
using BlazorWebApp.Data;
using BlazorWebApp.Services;

namespace BlazorWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategories();
            return Ok(categories);
        }

        // GET: api/Category/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        // POST: api/Category
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id = await _categoryService.CreateCategory(category);
            return Ok(id);
        }

        // PUT: api/Category/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            if (id != category.Id)
            {
                return BadRequest("Category ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _categoryService.UpdateCategory(id, category);

            if (result) return NoContent();
            return NotFound();
        }

        // DELETE: api/Category/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategory(id);
            if (result) return NoContent();
            return NotFound();
        }
    }
}
