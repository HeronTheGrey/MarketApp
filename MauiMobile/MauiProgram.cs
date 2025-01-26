using Microsoft.Extensions.Logging;

namespace MauiMobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.Services.AddSingleton<HttpClient>(sp =>
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://10.0.2.2:5217/api/")
            };
            return httpClient;
        });
        builder.Services.AddSingleton<ProductListPage>();
        builder.Services.AddTransient<ProductDetailsPage>();
        builder.Services.AddTransient<AddProductPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}