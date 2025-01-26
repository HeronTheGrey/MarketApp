using System.Net.Http.Json;
using MauiMarket.Shared.Models;

namespace MauiMobile;

public partial class ProductListPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public ProductListPage(HttpClient httpClient)
    {
        InitializeComponent();
        _httpClient = httpClient;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var products = await _httpClient.GetFromJsonAsync<List<Product>>("products");
        ProductListView.ItemsSource = products;
    }

    private async void OnProductSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Product selectedProduct)
        {
            await Navigation.PushAsync(new ProductDetailsPage(selectedProduct, _httpClient));
        }
    }

    private async void OnAddProductClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddProductPage(_httpClient));
    }
}