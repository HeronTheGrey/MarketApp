namespace MauiMarket.Shared.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } // Nazwa produktu
    public string Description { get; set; } // Opis produktu
    public decimal Price { get; set; } // Cena
    public int Stock { get; set; } // Ilość w magazynie
    public int CategoryId { get; set; } // Klucz obcy do kategorii
    public Category Category { get; set; } // Nawigacja do kategorii
}
