using System.Net.Http.Json;
using MauiMarket.Shared.Models;

namespace MauiMobile;

public partial class ProductListPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private List<Product> allProducts = new List<Product>();
    
    public ProductListPage(HttpClient httpClient)
    {
        InitializeComponent();
        _httpClient = httpClient;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        allProducts = await _httpClient.GetFromJsonAsync<List<Product>>("products");
        ProductListView.ItemsSource = allProducts;
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
    
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue.ToLower();
        ProductListView.ItemsSource = allProducts
            .Where(p => p.Name.ToLower().Contains(searchText))
            .ToList();
    }
}