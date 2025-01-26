using System.Net.Http.Json;
using MauiMarket.Shared.Models;

namespace MauiMobile;

public partial class ProductDetailsPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private Product _product;

    public ProductDetailsPage(Product product, HttpClient httpClient)
    {
        InitializeComponent();
        _httpClient = httpClient;
        _product = product;

        BindingContext = _product;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var response = await _httpClient.PutAsJsonAsync($"products/{_product.Id}", _product);

        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Success", "Product updated successfully", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", "Failed to update product", "OK");
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var response = await _httpClient.DeleteAsync($"products/{_product.Id}");

        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Success", "Product deleted successfully", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", "Failed to delete product", "OK");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}