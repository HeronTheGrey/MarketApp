using System.Net.Http.Json;
using MauiMarket.Shared.Models;

namespace MauiMobile;

public partial class AddProductPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private List<Category> categories;
    public AddProductPage(HttpClient httpClient)
    {
        InitializeComponent();
        _httpClient = httpClient;
        LoadCategories();
    }

    private async void LoadCategories()
    {
        try
        {
            categories = await _httpClient.GetFromJsonAsync<List<Category>>("category");
            CategoryPicker.ItemsSource = categories;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load categories: {ex.Message}", "OK");
        }
    }
    
    private async void OnAddProductClicked(object sender, EventArgs e)
    {
        if (CategoryPicker.SelectedItem is not Category selectedCategory)
        {
            await DisplayAlert("Error", "Please select a category.", "OK");
            return;
        }
        
        if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
            string.IsNullOrWhiteSpace(PriceEntry.Text))
        {
            await DisplayAlert("Error", "Please fill out all required fields.", "OK");
            return;
        }
        
        var newProduct = new Product
        {
            Name = NameEntry.Text,
            Description = DescriptionEditor.Text,
            Price = decimal.TryParse(PriceEntry.Text, out var price) ? price : 0,
            CategoryId = selectedCategory.Id
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("products", newProduct);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Success", "Product added successfully.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Failed to add product.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}