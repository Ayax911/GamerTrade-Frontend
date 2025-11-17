using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace GamerTrade.Services
{
    /// <summary>
    /// Interceptor HTTP que agrega el token JWT a todas las peticiones
    /// </summary>
    public class CustomAuthorizationHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthorizationHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Leer el token de localStorage
            var token = await _localStorage.GetItemAsync<string>("authToken");

            // Si existe token, agregarlo al header Authorization
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            // Continuar con la petición
            return await base.SendAsync(request, cancellationToken);
        }
    }
}