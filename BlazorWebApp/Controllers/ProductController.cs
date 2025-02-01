using System.Net.Mail;
using BlazorWebApp.Data;
using MauiMarket.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorWebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IEmailSender _emailSender;

    public ProductsController(AppDbContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    // GET: api/products
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] int? categoryId = null)
    {
        var productsQuery = _context.Products.AsQueryable();

        if (categoryId.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
        }

        var products = await productsQuery.ToListAsync();
        return Ok(products);
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
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

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
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

        try
        {
            _context.Update(product);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Products.Any(p => p.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/products/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound(new { message = "Produkt nie istnieje w bazie danych." });
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        
        var users = await _context.Users.ToListAsync();
        if (users == null || users.Count == 0)
        {
            return Ok(new { message = "Produkt usunięty, ale brak użytkowników do powiadomienia." });
        }
        var subject = "Usunięcie produktu";
        var message = $"Produkt {product.Name} został usunięty z oferty.";

        if (_emailSender == null)
        {
            return StatusCode(500, "Błąd systemowy: EmailSender nie został poprawnie zainicjalizowany.");
        }
        
        foreach (var user in users)
        {
            if (!string.IsNullOrWhiteSpace(user.Email) && IsValidEmail(user.Email))
            {
                await _emailSender.SendEmailAsync(user.Email, subject, message);
            }
        }
        
        return NoContent();
    }
    
    private bool IsValidEmail(string email)
    {
        try
        {
            var mailAddress = new MailAddress(email);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
