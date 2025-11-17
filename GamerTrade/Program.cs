/*using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GamerTrade;
using GamerTrade.Services;
using GamerTrade.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. Configurar LocalStorage
builder.Services.AddBlazoredLocalStorage();

// 2. Registrar CustomAuthStateProvider
builder.Services.AddScoped<CustomAuthStateProvider>();

// 3. Configurar HttpClient con el handler de autorización
builder.Services.AddScoped(sp =>
{
    // Crear el handler personalizado
    var localStorage = sp.GetRequiredService<ILocalStorageService>();
    var handler = new CustomAuthorizationHandler(localStorage)
    {
        InnerHandler = new HttpClientHandler()
    };

    // Crear HttpClient con el handler
    var client = new HttpClient(handler)
    {
        // ⚠️ CAMBIA ESTO A LA URL DE TU API
        BaseAddress = new Uri("https://localhost:5218/")
    };

    return client;
});

// 4. Registrar AuthenticationStateProvider
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());

// 5. Habilitar autorización
builder.Services.AddAuthorizationCore();

// 6. Registrar servicios de la aplicación
builder.Services.AddScoped<IAuthService>(sp =>
{
    var http = sp.GetRequiredService<HttpClient>();
    var localStorage = sp.GetRequiredService<ILocalStorageService>();
    var authProvider = sp.GetRequiredService<CustomAuthStateProvider>();

    return new AuthService(http, localStorage, authProvider);
});

await builder.Build().RunAsync();*/

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GamerTrade;
using GamerTrade.Services;
using GamerTrade.Services.Interfaces;
using GamerTrade.Services.Implementaciones;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ============================================
// 1. LocalStorage (base para todo)
// ============================================
builder.Services.AddBlazoredLocalStorage();

// ============================================
// 2. HttpClient con JWT Handler
// ============================================
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


// Autenticación

builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddAuthorizationCore();


//  Servicios de la Aplicación



builder.Services.AddScoped<IApiService, ApiService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJuegoService, JuegoService>();
builder.Services.AddScoped<ICarritoService, CarritoService>();



await builder.Build().RunAsync();