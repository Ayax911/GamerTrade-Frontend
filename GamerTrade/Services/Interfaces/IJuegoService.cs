using GamerTrade.Models.DTO;

namespace GamerTrade.Services.Interfaces
{
    public interface IJuegoService
    {

        Task<List<JuegoDTO>> ObtenerTodosAsync();
        Task<JuegoDTO?> ObtenerPorIdAsync(int id);
        Task<List<JuegoDTO>> BuscarAsync(string termino);
        Task<List<JuegoDTO>> ObtenerPorCategoriaAsync(int categoriaId);
        Task<List<JuegoDTO>> OrdenarPorPrecioAsync(bool ascendente = true);
        Task<List<CategoriaDTO>> ObtenerCategoriasAsync();
    }
}