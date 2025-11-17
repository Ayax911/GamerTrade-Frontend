using GamerTrade.Models.DTO;

namespace GamerTrade.Services.Interfaces
{
    public interface ICarritoService
    {
        Task<CarritoDTO> ObtenerCarritoAsync();
        Task<ResultadoCarritoDTO> AgregarItemAsync(int juegoId);
        Task<ResultadoCarritoDTO> EliminarItemAsync(int juegoId);
        Task<ResultadoCarritoDTO> VaciarCarritoAsync();
        Task<int> ObtenerConteoItemsAsync();
        event Action OnCarritoActualizado;
        void NotificarActualizacion();
    }

}
