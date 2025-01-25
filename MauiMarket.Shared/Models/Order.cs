namespace MauiMarket.Shared.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } // Data zamówienia
    public decimal TotalAmount { get; set; } // Całkowita kwota
    public int UserId { get; set; } // Klucz obcy do użytkownika
}
