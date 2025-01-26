using System.ComponentModel.DataAnnotations.Schema;

namespace MauiMarket.Shared.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } // Nazwa produktu
    public string Description { get; set; } // Opis produktu
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; } // Cena
    public int Stock { get; set; } // Ilość w magazynie
    public int CategoryId { get; set; } // Klucz obcy do kategorii
}
