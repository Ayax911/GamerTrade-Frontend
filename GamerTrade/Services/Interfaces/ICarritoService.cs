using GamerTrade.Models.DTO;

namespace GamerTrade.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de gestión del carrito de compras
    /// </summary>
    public interface ICarritoService
    {
        /// <summary>
        /// Agrega un juego al carrito del usuario
        /// </summary>
        Task<RespuestaCarritoDTO> AgregarAlCarritoAsync(int juegoId, string correoUsuario);

        /// <summary>
        /// Obtiene todos los items del carrito del usuario
        /// </summary>
        Task<List<ItemCarritoDTO>> ObtenerItemsCarritoAsync(string correoUsuario);

        /// <summary>
        /// Elimina un juego específico del carrito
        /// </summary>
        Task<RespuestaCarritoDTO> EliminarDelCarritoAsync(int juegoId, string correoUsuario);

        /// <summary>
        /// Vacía completamente el carrito del usuario
        /// </summary>
        Task<RespuestaCarritoDTO> VaciarCarritoAsync(string correoUsuario);

        /// <summary>
        /// Obtiene el resumen del carrito (total, cantidad de items)
        /// </summary>
        Task<ResumenCarritoDTO> ObtenerResumenCarritoAsync(string correoUsuario);

        /// <summary>
        /// Obtiene solo la cantidad de items en el carrito
        /// </summary>
        Task<int> ObtenerCantidadItemsAsync(string correoUsuario);
    }
}