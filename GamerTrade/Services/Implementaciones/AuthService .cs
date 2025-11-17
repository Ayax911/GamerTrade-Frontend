using System.Net.Http.Json;
using GamerTrade.Models.DTO;
using GamerTrade.Services.Interfaces;
using Blazored.LocalStorage;
using System.Text.Json;

namespace GamerTrade.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly CustomAuthStateProvider _authStateProvider;
        private readonly IApiService _apiService;

        public AuthService(
            HttpClient http,
            ILocalStorageService localStorage,
            CustomAuthStateProvider authStateProvider,
            IApiService apiService)
        {
            _http = http;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
            _apiService = apiService;
        }

        public async Task<(bool Success, string Message)> LoginAsync(LoginRequest request)
        {
            try
            {
                var url = $"api/Auth/verificar" +
                         $"?tabla=Usuario" +
                         $"&esquema=dbo" +
                         $"&campoUsuario=Correo" +
                         $"&campoContrasena=Contraseña" +
                         $"&valorUsuario={Uri.EscapeDataString(request.Correo)}" +
                         $"&valorContrasena={Uri.EscapeDataString(request.Password)}";

                Console.WriteLine($"🔵 Intentando login: {request.Correo}");

                var response = await _http.PostAsync(url, null);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<LoginResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (!string.IsNullOrEmpty(result?.Token))
                    {
                        await _localStorage.SetItemAsync("authToken", result.Token);
                        await _localStorage.SetItemAsync("userEmail", request.Correo);

                        _authStateProvider.NotificarUsuarioAutenticado(result.Token);

                        Console.WriteLine($"✅ Login exitoso para: {request.Correo}");
                        return (true, result.Mensaje ?? "Login exitoso");
                    }
                }

                Console.WriteLine($"❌ Login fallido: {content}");
                return (false, "Credenciales incorrectas");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en LoginAsync: {ex.Message}");
                return (false, "Error al conectar con el servidor");
            }
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("userEmail");
            _authStateProvider.NotificarUsuarioDesconectado();
            
            Console.WriteLine($"✅ Sesión cerrada");
        }

        public async Task<UsuarioDTO?> ObtenerUsuarioActualAsync()
        {
            try
            {
                var email = await _localStorage.GetItemAsync<string>("userEmail");
                
                if (string.IsNullOrEmpty(email))
                    return null;

                // Usa ApiService para obtener el usuario
                return await _apiService.ObtenerPorClaveAsync<UsuarioDTO>("Usuario", "Correo", email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error obteniendo usuario actual: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> EstaAutenticadoAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            return !string.IsNullOrEmpty(token);
        }

        public async Task<bool> RegistrarAsync(CrearUsuarioDTO request)
        {
            try
            {
                var datos = new Dictionary<string, object?>
                {
                    { "Apodo", request.Apodo },
                    { "Correo", request.Correo },
                    { "Contraseña", request.Contraseña },
                    { "RolID", request.RolID },
                    { "Saldo", request.Saldo }
                };

                // Usa ApiService para crear el usuario
                return await _apiService.CrearAsync("Usuario", datos, camposEncriptar: "Contraseña");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en RegistrarAsync: {ex.Message}");
                return false;
            }
        }
    }
}