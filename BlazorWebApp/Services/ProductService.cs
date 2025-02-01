using System.Net.Mail;
using BlazorWebApp.Data;
using MauiMarket.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorWebApp.Services;

public class ProductService
{
    private readonly AppDbContext _context;
    private readonly IEmailSender _emailSender;
    
    public ProductService(AppDbContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    public List<Product> GetAllProductsBeCategory(int? categoryId)
    {
        var productsQuery = _context.Products.AsQueryable();

        if (categoryId.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
        }

        var products = productsQuery.ToList();
        return products;
    }
    
    public Product? GetProductById(int id)
    {
        var product = _context.Products.Find(id);
        return product ?? null;
    }
    
    public async Task<int> AddProduct(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product.Id;
    }
    
    public async Task<bool> UpdateProduct(int id, Product product)
    {
        try
        {
            _context.Update(product);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Products.Any(p => p.Id == id))
            {
                return false;
            }
            throw;
        }
        return true;
    }
    
    public async Task<bool> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return false;
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        
        var users = await _context.Users.ToListAsync();
        if (users == null || users.Count == 0)
        {
            return true;
        }
        var subject = "Usunięcie produktu";
        var message = $"Produkt {product.Name} został usunięty z oferty.";
        
        foreach (var user in users.Where(user => !string.IsNullOrWhiteSpace(user.Email) && IsValidEmail(user.Email)))
        {
            await _emailSender.SendEmailAsync(user.Email, subject, message);
        }
        
        return true;
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