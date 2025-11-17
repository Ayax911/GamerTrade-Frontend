using GamerTrade.Models.DTO;
using GamerTrade.Services.Interfaces;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace GamerTrade.Services.Implementaciones
{
    public class JuegoService : IJuegoService
    {
        private readonly IApiService _apiService;

        public JuegoService(IApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Obtiene todos los juegos disponibles
        /// </summary>
        public async Task<List<JuegoDTO>> ObtenerTodosAsync()
        {
            try
            {
                var juegos = await _apiService.ListarAsync<JuegoDTO>("Juego");
                return juegos ?? new List<JuegoDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ObtenerTodosAsync: {ex.Message}");
                return new List<JuegoDTO>();
            }
        }

        /// <summary>
        /// Obtiene un juego por su ID
        /// </summary>
        public async Task<JuegoDTO?> ObtenerPorIdAsync(int id)
        {
            try
            {
                return await _apiService.ObtenerPorClaveAsync<JuegoDTO>(
                    "Juego",
                    "JuegoID",
                    id.ToString()
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ObtenerPorIdAsync: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Busca juegos por título
        /// </summary>
        public async Task<List<JuegoDTO>> BuscarAsync(string termino)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termino))
                    return await ObtenerTodosAsync();

                var juegos = await ObtenerTodosAsync();

                return juegos.Where(j =>
                    j.Titulo.Contains(termino, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en BuscarAsync: {ex.Message}");
                return new List<JuegoDTO>();
            }
        }

        /// <summary>
        /// Filtra juegos por categoría
        /// </summary>
        public async Task<List<JuegoDTO>> ObtenerPorCategoriaAsync(int categoriaId)
        {
            try
            {
                if (categoriaId == 0)
                    return await ObtenerTodosAsync();

                var juegos = await ObtenerTodosAsync();

                return juegos.Where(j => j.CategoriaID == categoriaId).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ObtenerPorCategoriaAsync: {ex.Message}");
                return new List<JuegoDTO>();
            }
        }

        /// <summary>
        /// Ordena juegos por precio
        /// </summary>
        public async Task<List<JuegoDTO>> OrdenarPorPrecioAsync(bool ascendente = true)
        {
            try
            {
                var juegos = await ObtenerTodosAsync();

                return ascendente
                    ? juegos.OrderBy(j => j.Precio).ToList()
                    : juegos.OrderByDescending(j => j.Precio).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en OrdenarPorPrecioAsync: {ex.Message}");
                return new List<JuegoDTO>();
            }
        }

        /// <summary>
        /// Obtiene todas las categorías disponibles
        /// </summary>
        public async Task<List<CategoriaDTO>> ObtenerCategoriasAsync()
        {
            try
            {
                var categorias = await _apiService.ListarAsync<CategoriaDTO>("Categoria");
                return categorias ?? new List<CategoriaDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ObtenerCategoriasAsync: {ex.Message}");
                return new List<CategoriaDTO>();
            }
        }
    }
}