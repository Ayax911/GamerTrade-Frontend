namespace GamerTrade.Services.Implementaciones
{
    using GamerTrade.Models.DTO;
    using GamerTrade.Services.Interfaces;
    using System.Net.Http.Json;

    public class CarritoService : ICarritoService
    {
        private readonly HttpClient _httpClient;
        private CarritoDTO _carritoEnCache = new();

        public event Action OnCarritoActualizado;

        public CarritoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CarritoDTO> ObtenerCarritoAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/carrito");

                if (response.IsSuccessStatusCode)
                {
                    _carritoEnCache = await response.Content.ReadFromJsonAsync<CarritoDTO>() ?? new CarritoDTO();
                    return _carritoEnCache;
                }

                return _carritoEnCache;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error obteniendo carrito: {ex.Message}");
                return _carritoEnCache;
            }
        }

        public async Task<ResultadoCarritoDTO> AgregarItemAsync(int juegoId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/carrito/agregar/{juegoId}", null);

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResultadoCarritoDTO>();
                    await ObtenerCarritoAsync(); // Sincronizar caché
                    NotificarActualizacion();
                    return resultado;
                }

                return new ResultadoCarritoDTO
                {
                    Resultado = 0,
                    Mensaje = $"Error: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ResultadoCarritoDTO
                {
                    Resultado = 0,
                    Mensaje = ex.Message
                };
            }
        }

        public async Task<ResultadoCarritoDTO> EliminarItemAsync(int juegoId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/carrito/eliminar/{juegoId}");

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResultadoCarritoDTO>();
                    await ObtenerCarritoAsync();
                    NotificarActualizacion();
                    return resultado;
                }

                return new ResultadoCarritoDTO
                {
                    Resultado = 0,
                    Mensaje = "Error eliminando item"
                };
            }
            catch (Exception ex)
            {
                return new ResultadoCarritoDTO
                {
                    Resultado = 0,
                    Mensaje = ex.Message
                };
            }
        }

        public async Task<ResultadoCarritoDTO> VaciarCarritoAsync()
        {
            try
            {
                var response = await _httpClient.DeleteAsync("api/carrito/vaciar");

                if (response.IsSuccessStatusCode)
                {
                    _carritoEnCache = new CarritoDTO { Items = new() };
                    NotificarActualizacion();
                    return new ResultadoCarritoDTO
                    {
                        Resultado = 1,
                        Mensaje = "Carrito vaciado"
                    };
                }

                return new ResultadoCarritoDTO
                {
                    Resultado = 0,
                    Mensaje = "Error vaciando carrito"
                };
            }
            catch (Exception ex)
            {
                return new ResultadoCarritoDTO
                {
                    Resultado = 0,
                    Mensaje = ex.Message
                };
            }
        }

        public async Task<int> ObtenerConteoItemsAsync()
        {
            var carrito = await ObtenerCarritoAsync();
            return carrito?.Items?.Count ?? 0;
        }

        public void NotificarActualizacion()
        {
            OnCarritoActualizado?.Invoke();
        }
    }
}