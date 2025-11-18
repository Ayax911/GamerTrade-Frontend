

using Blazored.LocalStorage;
using GamerTrade;
using GamerTrade.Services;
using GamerTrade.Services.Implementaciones;
using GamerTrade.Services.Interfaces;
using GamerTrade.State;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<HttpClient>(sp =>
{
    var localStorage = sp.GetRequiredService<ILocalStorageService>();

    var handler = new CustomAuthorizationHandler(localStorage)
    {
        InnerHandler = new HttpClientHandler()
    };

    var client = new HttpClient(handler)
    {
        
        BaseAddress = new Uri("http://localhost:5218/")
    };

    return client;
});

builder.Services.AddBlazoredLocalStorage();

// Autenticación

builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddAuthorizationCore();


builder.Services.AddSingleton<AppState>();
builder.Services.AddTransient<OrdenadorDeJuegos>();




builder.Services.AddScoped<IApiService, ApiService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJuegoService, JuegoService>();
builder.Services.AddScoped<ICarritoService, CarritoService>();




await builder.Build().RunAsync();