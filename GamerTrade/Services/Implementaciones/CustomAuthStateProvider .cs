using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;

namespace GamerTrade.Services
{
    /// <summary>
    /// Proveedor de estado de autenticación personalizado para JWT
    /// </summary>
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        /// <summary>
        /// Obtiene el estado de autenticación actual
        /// Se llama automáticamente cuando se usa <AuthorizeView> o [Authorize]
        /// </summary>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Leer token de localStorage
            var token = await _localStorage.GetItemAsync<string>("authToken");

            // Si no hay token, usuario anónimo
            if (string.IsNullOrEmpty(token))
            {
                var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
                return new AuthenticationState(anonymous);
            }

            // Parsear claims del JWT
            var claims = ParseClaimsFromJwt(token);

            // Crear identidad autenticada
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        /// <summary>
        /// Notifica que el usuario se autenticó (después del login exitoso)
        /// </summary>
        public void NotificarUsuarioAutenticado(string token)
        {
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            var authState = Task.FromResult(new AuthenticationState(user));

            // Notificar a todos los componentes que el estado cambió
            NotifyAuthenticationStateChanged(authState);
        }

        /// <summary>
        /// Notifica que el usuario cerró sesión
        /// </summary>
        public void NotificarUsuarioDesconectado()
        {
            var identity = new ClaimsIdentity(); // Identidad vacía
            var user = new ClaimsPrincipal(identity);

            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }

        /// <summary>
        /// Extrae los claims del token JWT
        /// </summary>
        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);

                // Extraer todos los claims del token
                claims.AddRange(token.Claims);

                // Ejemplo de claims comunes en JWT:
                // - "sub" o "unique_name": email del usuario
                // - "role": rol del usuario
                // - "exp": expiración
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parseando JWT: {ex.Message}");
            }

            return claims;
        }
    }
}