using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace MauiMobile
{
    public partial class LoginPage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("http://10.0.2.2:5217/api/") };

        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                ErrorMessage.Text = "Wprowadź nazwę użytkownika i hasło!";
                ErrorMessage.IsVisible = true;
                return;
            }

            var loginDto = new
            {
                Username = UsernameEntry.Text,
                Password = PasswordEntry.Text
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();

                    // Zapisz token JWT w SecureStorage
                    await SecureStorage.SetAsync("authToken", token);

                    await DisplayAlert("Sukces", "Zalogowano pomyślnie", "OK");

                    // Przekierowanie do MainPage
                    await Navigation.PushAsync(new ProductListPage(_httpClient));
                }
                else
                {
                    ErrorMessage.Text = "Nieprawidłowa nazwa użytkownika lub hasło.";
                    ErrorMessage.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", "Błąd połączenia: " + ex.Message, "OK");
            }
        }
    }
}