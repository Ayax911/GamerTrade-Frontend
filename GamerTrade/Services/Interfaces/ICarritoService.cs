using GamerTrade.Models.DTO;

namespace GamerTrade.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de gestión del carrito de compras
    /// </summary>
    public interface ICarritoService
    {
        /// <summary>
        /// Obtiene el carrito del usuario por su email
        /// </summary>
        Task<CarritoDTO?> ObtenerCarritoAsync(string correoUsuario);

        /// <summary>
        /// Agrega un juego al carrito del usuario
        /// Si el juego ya existe en el carrito, no se agrega nuevamente
        /// </summary>
        Task<bool> AgregarAlCarritoAsync(string correoUsuario, int juegoId);

        /// <summary>
        /// Elimina un juego del carrito del usuario
        /// </summary>
        Task<bool> EliminarDelCarritoAsync(string correoUsuario, int juegoId);

        /// <summary>
        /// Obtiene todos los ítems del carrito del usuario
        /// </summary>
        Task<List<ItemCarritoDTO>> ObtenerItemsCarritoAsync(string correoUsuario);

        /// <summary>
        /// Calcula el total del carrito
        /// </summary>
        Task<decimal> ObtenerTotalCarritoAsync(string correoUsuario);
    }

}