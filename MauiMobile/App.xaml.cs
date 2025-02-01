namespace MauiMobile;

public partial class App : Application
{
    private readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("http://10.0.2.2:5217/api/") };
    public App()
    {
        InitializeComponent();

        MainPage = new NavigationPage(CheckLoginStatus());
    }
    
    private Page CheckLoginStatus()
    {
        var token = SecureStorage.GetAsync("authToken").Result;
        return string.IsNullOrEmpty(token) ? new LoginPage() : new ProductListPage(_httpClient);
    }
}