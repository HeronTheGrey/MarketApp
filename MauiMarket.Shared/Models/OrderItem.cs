namespace MauiMarket.Shared.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; } // Klucz obcy do zamówienia
    public int ProductId { get; set; } // Klucz obcy do produktu
    public int Quantity { get; set; } // Ilość
    public decimal UnitPrice { get; set; } // Cena jednostkowa
    public decimal TotalPrice { get; set; } // Cena łączna (Quantity * UnitPrice)
}
