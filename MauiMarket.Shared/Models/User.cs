namespace MauiMarket.Shared.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } // Imię
    public string LastName { get; set; } // Nazwisko
    public string Email { get; set; } // Email
    public string PasswordHash { get; set; } // Hasło (zaszyfrowane)
    public string Role { get; set; } // Rola (np. "Customer", "Admin")
    public ICollection<Order> Orders { get; set; } // Lista zamówień użytkownika
}
