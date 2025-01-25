namespace MauiMarket.Shared.Models;

public class Address
{
    public int Id { get; set; }
    public string Street { get; set; } // Ulica
    public string City { get; set; } // Miasto
    public string State { get; set; } // Województwo
    public string PostalCode { get; set; } // Kod pocztowy
    public string Country { get; set; } // Kraj
    public int UserId { get; set; } // Klucz obcy do użytkownika
}
