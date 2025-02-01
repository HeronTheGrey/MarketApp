using BlazorWebApp.Data;
using MauiMarket.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorWebApp.Services;

public class CategoryService
{
    private readonly AppDbContext _context;
    public CategoryService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Category>> GetAllCategories()
    {
        var categories = await _context.Categories.ToListAsync();
        return categories;
    }
    
    public async Task<Category?> GetCategoryById(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        return category ?? null;
    }
    
    public async Task<int> CreateCategory(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category.Id;
    }
    
    public async Task<bool> UpdateCategory(int id, Category category)
    {
        _context.Entry(category).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoryExists(id))
            {
                return false;
            }
            throw;
        }

        return true;
    }
    
    public async Task<bool> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return false;
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }
    
    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}