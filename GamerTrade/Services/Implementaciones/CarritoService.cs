using GamerTrade.Models.DTO;
using GamerTrade.Services.Interfaces;

namespace GamerTrade.Services.Implementaciones
{
    /// <summary>
    /// Servicio para la gestión del carrito de compras
    /// Usa stored procedures a través del ApiService
    /// </summary>
    public class CarritoService : ICarritoService
    {
        private readonly IApiService _apiService;

        public CarritoService(IApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Agrega un juego al carrito del usuario
        /// </summary>
        public async Task<RespuestaCarritoDTO> AgregarAlCarritoAsync(int juegoId, string correoUsuario)
        {
            try
            {
                Console.WriteLine($"🛒 Agregando juego {juegoId} al carrito de {correoUsuario}");

                var consulta = "EXEC sp_AgregarItemCarrito @CorreoUsuario, @JuegoID";
                var parametros = new Dictionary<string, object>
                {
                    { "@CorreoUsuario", correoUsuario },
                    { "@JuegoID", juegoId }
                };

                var resultado = await _apiService.EjecutarConsultaAsync<RespuestaCarritoDTO>(
                    consulta,
                    parametros
                );

                if (resultado != null && resultado.Count > 0)
                {
                    Console.WriteLine($"✅ Respuesta: {resultado[0].Mensaje}");
                    return resultado[0];
                }

                return new RespuestaCarritoDTO
                {
                    Mensaje = "Error al agregar el juego al carrito",
                    Exito = 0
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en AgregarAlCarritoAsync: {ex.Message}");
                return new RespuestaCarritoDTO
                {
                    Mensaje = $"Error: {ex.Message}",
                    Exito = 0
                };
            }
        }

        /// <summary>
        /// Obtiene todos los items del carrito del usuario
        /// </summary>
        public async Task<List<ItemCarritoDTO>> ObtenerItemsCarritoAsync(string correoUsuario)
        {
            try
            {
                Console.WriteLine($"📦 Obteniendo items del carrito de {correoUsuario}");

                var consulta = "EXEC sp_ObtenerItemsCarrito @CorreoUsuario";
                var parametros = new Dictionary<string, object>
                {
                    { "@CorreoUsuario", correoUsuario }
                };

                var resultado = await _apiService.EjecutarConsultaAsync<ItemCarritoDTO>(
                    consulta,
                    parametros
                );

                Console.WriteLine($"✅ {resultado?.Count ?? 0} items en el carrito");
                return resultado ?? new List<ItemCarritoDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ObtenerItemsCarritoAsync: {ex.Message}");
                return new List<ItemCarritoDTO>();
            }
        }

        /// <summary>
        /// Elimina un juego del carrito
        /// </summary>
        public async Task<RespuestaCarritoDTO> EliminarDelCarritoAsync(int juegoId, string correoUsuario)
        {
            try
            {
                Console.WriteLine($"🗑️ Eliminando juego {juegoId} del carrito de {correoUsuario}");

                var consulta = "EXEC sp_EliminarItemCarrito @CorreoUsuario, @JuegoID";
                var parametros = new Dictionary<string, object>
                {
                    { "@CorreoUsuario", correoUsuario },
                    { "@JuegoID", juegoId }
                };

                var resultado = await _apiService.EjecutarConsultaAsync<RespuestaCarritoDTO>(
                    consulta,
                    parametros
                );

                if (resultado != null && resultado.Count > 0)
                {
                    Console.WriteLine($"✅ Respuesta: {resultado[0].Mensaje}");
                    return resultado[0];
                }

                return new RespuestaCarritoDTO
                {
                    Mensaje = "Error al eliminar el juego del carrito",
                    Exito = 0
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en EliminarDelCarritoAsync: {ex.Message}");
                return new RespuestaCarritoDTO
                {
                    Mensaje = $"Error: {ex.Message}",
                    Exito = 0
                };
            }
        }

        /// <summary>
        /// Vacía completamente el carrito del usuario
        /// </summary>
        public async Task<RespuestaCarritoDTO> VaciarCarritoAsync(string correoUsuario)
        {
            try
            {
                Console.WriteLine($"🧹 Vaciando carrito de {correoUsuario}");

                var consulta = "EXEC sp_VaciarCarrito @CorreoUsuario";
                var parametros = new Dictionary<string, object>
                {
                    { "@CorreoUsuario", correoUsuario }
                };

                var resultado = await _apiService.EjecutarConsultaAsync<RespuestaCarritoDTO>(
                    consulta,
                    parametros
                );

                if (resultado != null && resultado.Count > 0)
                {
                    Console.WriteLine($"✅ Respuesta: {resultado[0].Mensaje}");
                    return resultado[0];
                }

                return new RespuestaCarritoDTO
                {
                    Mensaje = "Error al vaciar el carrito",
                    Exito = 0
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en VaciarCarritoAsync: {ex.Message}");
                return new RespuestaCarritoDTO
                {
                    Mensaje = $"Error: {ex.Message}",
                    Exito = 0
                };
            }
        }

        /// <summary>
        /// Obtiene el resumen del carrito (total y cantidad de items)
        /// </summary>
        public async Task<ResumenCarritoDTO> ObtenerResumenCarritoAsync(string correoUsuario)
        {
            try
            {
                Console.WriteLine($"📊 Obteniendo resumen del carrito de {correoUsuario}");

                // Obtener resumen básico del carrito
                var consultaResumen = "EXEC sp_ObtenerResumenCarrito @CorreoUsuario";
                var parametros = new Dictionary<string, object>
                {
                    { "@CorreoUsuario", correoUsuario }
                };

                var resumen = await _apiService.EjecutarConsultaAsync<ResumenCarritoDTO>(
                    consultaResumen,
                    parametros
                );

                if (resumen != null && resumen.Count > 0)
                {
                    var resumenCarrito = resumen[0];

                    // Obtener items del carrito
                    resumenCarrito.Items = await ObtenerItemsCarritoAsync(correoUsuario);

                    Console.WriteLine($"✅ Resumen: {resumenCarrito.CantidadItems} items, Total: ${resumenCarrito.Total}");
                    return resumenCarrito;
                }

                // Si no hay carrito, devolver uno vacío
                Console.WriteLine($"⚠️ No se encontró carrito para {correoUsuario}");
                return new ResumenCarritoDTO
                {
                    CarritoID = Guid.Empty,
                    Total = 0,
                    CantidadItems = 0,
                    Correo_usuario = correoUsuario,
                    Items = new List<ItemCarritoDTO>()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ObtenerResumenCarritoAsync: {ex.Message}");
                return new ResumenCarritoDTO
                {
                    CarritoID = Guid.Empty,
                    Total = 0,
                    CantidadItems = 0,
                    Correo_usuario = correoUsuario,
                    Items = new List<ItemCarritoDTO>()
                };
            }
        }

        /// <summary>
        /// Obtiene solo la cantidad de items en el carrito (más eficiente)
        /// </summary>
        public async Task<int> ObtenerCantidadItemsAsync(string correoUsuario)
        {
            try
            {
                var resumen = await ObtenerResumenCarritoAsync(correoUsuario);
                return resumen.CantidadItems;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ObtenerCantidadItemsAsync: {ex.Message}");
                return 0;
            }
        }
    }
}