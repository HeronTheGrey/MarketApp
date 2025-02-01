using BlazorWebApp.Data;
using BlazorWebApp.Services;
using MauiMarket.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    // GET: api/products
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] int? categoryId = null)
    {
        var products = _productService.GetAllProductsBeCategory(categoryId);
        return Ok(products);
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = _productService.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    // POST: api/products
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var id = await _productService.AddProduct(product);
        return Ok(id);
    }

    // PUT: api/products/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
    {
        if (id != product.Id)
        {
            return BadRequest("Product ID mismatch");
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest(new { Message = "Invalid model state", Errors = errors });
        }

        var result = await _productService.UpdateProduct(id, product);
        if (result) return NoContent();
        return NotFound();
    }

    // DELETE: api/products/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProduct(id);
        if (result) return NoContent();
        return NotFound(new { message = "Produkt nie istnieje w bazie danych." });
    }
}
