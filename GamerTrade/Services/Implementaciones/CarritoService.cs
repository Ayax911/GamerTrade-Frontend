using Blazored.LocalStorage;
using GamerTrade.Models.DTO;
using GamerTrade.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace GamerTrade.Services.Implementaciones
{
    public class CarritoService : ICarritoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly JsonSerializerOptions _jsonOptions;

        public CarritoService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };
        }

        /// <summary>
        /// Obtiene el email del usuario autenticado desde localStorage
        /// </summary>
        private async Task<string?> ObtenerEmailUsuarioAsync()
        {
            try
            {
                return await _localStorage.GetItemAsync<string>("userEmail");
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Agrega un juego al carrito del usuario autenticado
        /// Retorna false si el juego ya existe o hay error
        /// </summary>
        public async Task<bool> AgregarAlCarritoAsync(string correoUsuario, int juegoId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(correoUsuario) || juegoId <= 0)
                {
                    Console.WriteLine("❌ Parámetros inválidos");
                    return false;
                }

                Console.WriteLine($"🛒 Agregando juego {juegoId} al carrito de {correoUsuario}");

                // 1. Obtener o crear carrito
                var carritoId = await ObtenerOCrearCarritoAsync(correoUsuario);
                if (carritoId == Guid.Empty)
                {
                    Console.WriteLine("❌ No se pudo obtener/crear el carrito");
                    return false;
                }

                // 2. Verificar si el juego ya está en el carrito
                var yaExiste = await VerificarJuegoEnCarritoAsync(carritoId, juegoId);
                if (yaExiste)
                {
                    Console.WriteLine($"⚠️ El juego {juegoId} ya está en el carrito");
                    return false;
                }

                // 3. Verificar que el juego existe
                var juegoExiste = await VerificarJuegoExisteAsync(juegoId);
                if (!juegoExiste)
                {
                    Console.WriteLine($"❌ El juego {juegoId} no existe");
                    return false;
                }

                // 4. Insertar el juego en ItemsCarrito
                var consultaInsert = @"
                    INSERT INTO ItemsCarrito (juego, Carrito)
                    VALUES (@JuegoID, @CarritoID)";

                var bodyInsert = new
                {
                    consulta = consultaInsert,
                    parametros = new
                    {
                        JuegoID = juegoId,
                        CarritoID = carritoId
                    }
                };

                var responseInsert = await _httpClient.PostAsJsonAsync(
                    "api/consultas/EjecutarConsulta", bodyInsert);

                if (!responseInsert.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Error al insertar juego en carrito");
                    return false;
                }

                // 5. Actualizar el total del carrito
                var consultaUpdate = @"
                    UPDATE Carrito
                    SET Valor_total = (
                        SELECT ISNULL(SUM(j.Precio), 0)
                        FROM ItemsCarrito ic
                        JOIN Juego j ON ic.juego = j.JuegoID
                        WHERE ic.Carrito = @CarritoID
                    )
                    WHERE UniqueID = @CarritoID";

                var bodyUpdate = new
                {
                    consulta = consultaUpdate,
                    parametros = new { CarritoID = carritoId }
                };

                await _httpClient.PostAsJsonAsync("api/consultas/EjecutarConsulta", bodyUpdate);

                Console.WriteLine($"✅ Juego {juegoId} agregado al carrito exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en AgregarAlCarritoAsync: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un juego del carrito del usuario
        /// </summary>
        public async Task<bool> EliminarDelCarritoAsync(string correoUsuario, int juegoId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(correoUsuario) || juegoId <= 0)
                {
                    Console.WriteLine("❌ Parámetros inválidos");
                    return false;
                }

                Console.WriteLine($"🗑️ Eliminando juego {juegoId} del carrito de {correoUsuario}");

                // 1. Obtener el carrito ID
                var carritoId = await ObtenerCarritoIdAsync(correoUsuario);
                if (carritoId == Guid.Empty)
                {
                    Console.WriteLine("❌ Carrito no encontrado");
                    return false;
                }

                // 2. Eliminar de ItemsCarrito
                var consultaDelete = @"
                    DELETE FROM ItemsCarrito
                    WHERE juego = @JuegoID AND Carrito = @CarritoID";

                var bodyDelete = new
                {
                    consulta = consultaDelete,
                    parametros = new
                    {
                        JuegoID = juegoId,
                        CarritoID = carritoId
                    }
                };

                var responseDelete = await _httpClient.PostAsJsonAsync(
                    "api/consultas/EjecutarConsulta", bodyDelete);

                if (!responseDelete.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Error al eliminar juego del carrito");
                    return false;
                }

                // 3. Actualizar el total del carrito
                var consultaUpdate = @"
                    UPDATE Carrito
                    SET Valor_total = (
                        SELECT ISNULL(SUM(j.Precio), 0)
                        FROM ItemsCarrito ic
                        JOIN Juego j ON ic.juego = j.JuegoID
                        WHERE ic.Carrito = @CarritoID
                    )
                    WHERE UniqueID = @CarritoID";

                var bodyUpdate = new
                {
                    consulta = consultaUpdate,
                    parametros = new { CarritoID = carritoId }
                };

                await _httpClient.PostAsJsonAsync("api/consultas/EjecutarConsulta", bodyUpdate);

                Console.WriteLine($"✅ Juego {juegoId} eliminado del carrito exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en EliminarDelCarritoAsync: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene todos los ítems del carrito del usuario
        /// </summary>
        public async Task<List<ItemCarritoDTO>> ObtenerItemsCarritoAsync(string correoUsuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(correoUsuario))
                {
                    Console.WriteLine("❌ Email del usuario no válido");
                    return new List<ItemCarritoDTO>();
                }

                Console.WriteLine($"📦 Obteniendo items del carrito para: {correoUsuario}");

                var consulta = @"
                    SELECT 
                        j.JuegoID,
                        j.Titulo,
                        j.Precio,
                        cat.Nombre AS CategoriaNombre,
                        j.Calificacion,
                        ISNULL(j.UrlImagen, '') AS UrlImagen
                    FROM ItemsCarrito ic
                    JOIN Carrito c ON ic.Carrito = c.UniqueID
                    JOIN Juego j ON ic.juego = j.JuegoID
                    JOIN Categoria cat ON j.CategoriaID = cat.CategoriaID
                    WHERE c.Correo_usuario = @Correo
                    ORDER BY j.Titulo";

                var body = new
                {
                    consulta = consulta,
                    parametros = new { Correo = correoUsuario }
                };

                var response = await _httpClient.PostAsJsonAsync("api/consultas/EjecutarConsulta", body);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Error HTTP: {response.StatusCode}");
                    return new List<ItemCarritoDTO>();
                }

                var json = await response.Content.ReadAsStringAsync();
                var resultado = JsonSerializer.Deserialize<JsonElement>(json, _jsonOptions);

                if (!resultado.TryGetProperty("Registros", out var registrosElement))
                {
                    Console.WriteLine("❌ No hay propiedad 'Registros' en la respuesta");
                    return new List<ItemCarritoDTO>();
                }

                var items = new List<ItemCarritoDTO>();

                foreach (var registro in registrosElement.EnumerateArray())
                {
                    var item = new ItemCarritoDTO
                    {
                        JuegoId = ObtenerInt(registro, "juegoID"),
                        Titulo = ObtenerString(registro, "titulo"),
                        Precio = ObtenerDecimal(registro, "precio"),
                        CategoriaNombre = ObtenerString(registro, "categoriaNombre"),
                        Calificacion = ObtenerDecimalNullable(registro, "calificacion"),
                        UrlImagen = ObtenerString(registro, "urlImagen"),
                        Cantidad = 1  // Siempre 1 porque no permitimos duplicados
                    };

                    items.Add(item);
                }

                Console.WriteLine($"✅ Se obtuvieron {items.Count} items del carrito");
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ObtenerItemsCarritoAsync: {ex.Message}");
                return new List<ItemCarritoDTO>();
            }
        }

        /// <summary>
        /// Obtiene el carrito completo del usuario
        /// </summary>
        public async Task<CarritoDTO?> ObtenerCarritoAsync(string correoUsuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(correoUsuario))
                {
                    Console.WriteLine("❌ Email del usuario no válido");
                    return null;
                }

                Console.WriteLine($"🛒 Obteniendo carrito para: {correoUsuario}");

                var consulta = "SELECT UniqueID, Correo_usuario, Valor_total FROM Carrito WHERE Correo_usuario = @Correo";
                var body = new
                {
                    consulta = consulta,
                    parametros = new { Correo = correoUsuario }
                };

                var response = await _httpClient.PostAsJsonAsync("api/consultas/EjecutarConsulta", body);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Carrito no encontrado");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var resultado = JsonSerializer.Deserialize<JsonElement>(json, _jsonOptions);

                if (!resultado.TryGetProperty("Registros", out var registrosElement))
                {
                    return null;
                }

                var registros = registrosElement.EnumerateArray().ToList();
                if (registros.Count == 0)
                {
                    Console.WriteLine($"⚠️ Carrito no encontrado para: {correoUsuario}");
                    return null;
                }

                var primerRegistro = registros[0];
                var carrito = new CarritoDTO
                {
                    UniqueID = Guid.Parse(ObtenerString(primerRegistro, "uniqueID")),
                    Correo_usuario = ObtenerString(primerRegistro, "correo_usuario"),
                    Items = await ObtenerItemsCarritoAsync(correoUsuario)
                };

                Console.WriteLine($"✅ Carrito obtenido con {carrito.Items.Count} items");
                return carrito;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ObtenerCarritoAsync: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Calcula el total del carrito
        /// </summary>
        public async Task<decimal> ObtenerTotalCarritoAsync(string correoUsuario)
        {
            try
            {
                var items = await ObtenerItemsCarritoAsync(correoUsuario);
                var total = items.Sum(i => i.Precio * i.Cantidad);
                Console.WriteLine($"💰 Total del carrito: ${total:F2}");
                return total;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ObtenerTotalCarritoAsync: {ex.Message}");
                return 0;
            }
        }

        // ==================== MÉTODOS PRIVADOS ====================

        /// <summary>
        /// Obtiene el ID del carrito o crea uno si no existe
        /// </summary>
        private async Task<Guid> ObtenerOCrearCarritoAsync(string correoUsuario)
        {
            var carritoId = await ObtenerCarritoIdAsync(correoUsuario);

            if (carritoId == Guid.Empty)
            {
                Console.WriteLine($"⚠️ Carrito no existe, creando nuevo...");

                var consultaCrear = "INSERT INTO Carrito (Correo_usuario, Valor_total) VALUES (@Correo, 0)";
                var bodyCrear = new
                {
                    consulta = consultaCrear,
                    parametros = new { Correo = correoUsuario }
                };

                var response = await _httpClient.PostAsJsonAsync("api/consultas/EjecutarConsulta", bodyCrear);

                if (response.IsSuccessStatusCode)
                {
                    carritoId = await ObtenerCarritoIdAsync(correoUsuario);
                }
            }

            return carritoId;
        }

        /// <summary>
        /// Obtiene solo el ID del carrito
        /// </summary>
        private async Task<Guid> ObtenerCarritoIdAsync(string correoUsuario)
        {
            try
            {
                var consulta = "SELECT UniqueID FROM Carrito WHERE Correo_usuario = @Correo";
                var body = new
                {
                    consulta = consulta,
                    parametros = new { Correo = correoUsuario }
                };

                var response = await _httpClient.PostAsJsonAsync("api/consultas/EjecutarConsulta", body);

                if (!response.IsSuccessStatusCode)
                    return Guid.Empty;

                var json = await response.Content.ReadAsStringAsync();
                var resultado = JsonSerializer.Deserialize<JsonElement>(json, _jsonOptions);

                if (!resultado.TryGetProperty("Registros", out var registrosElement))
                    return Guid.Empty;

                var registros = registrosElement.EnumerateArray().ToList();
                if (registros.Count == 0)
                    return Guid.Empty;

                var uniqueIdStr = ObtenerString(registros[0], "uniqueID");
                return Guid.TryParse(uniqueIdStr, out var guid) ? guid : Guid.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al obtener CarritoID: {ex.Message}");
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Verifica si un juego ya existe en el carrito
        /// </summary>
        private async Task<bool> VerificarJuegoEnCarritoAsync(Guid carritoId, int juegoId)
        {
            try
            {
                var consulta = @"
                    SELECT COUNT(*) AS Cantidad
                    FROM ItemsCarrito
                    WHERE Carrito = @CarritoID AND juego = @JuegoID";

                var body = new
                {
                    consulta = consulta,
                    parametros = new
                    {
                        CarritoID = carritoId,
                        JuegoID = juegoId
                    }
                };

                var response = await _httpClient.PostAsJsonAsync("api/consultas/EjecutarConsulta", body);

                if (!response.IsSuccessStatusCode)
                    return false;

                var json = await response.Content.ReadAsStringAsync();
                var resultado = JsonSerializer.Deserialize<JsonElement>(json, _jsonOptions);

                if (!resultado.TryGetProperty("Registros", out var registrosElement))
                    return false;

                var registros = registrosElement.EnumerateArray().ToList();
                if (registros.Count == 0)
                    return false;

                var cantidad = ObtenerInt(registros[0], "cantidad");
                return cantidad > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al verificar juego en carrito: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica si un juego existe en la BD
        /// </summary>
        private async Task<bool> VerificarJuegoExisteAsync(int juegoId)
        {
            try
            {
                var consulta = "SELECT JuegoID FROM Juego WHERE JuegoID = @JuegoID";
                var body = new
                {
                    consulta = consulta,
                    parametros = new { JuegoID = juegoId }
                };

                var response = await _httpClient.PostAsJsonAsync("api/consultas/EjecutarConsulta", body);

                if (!response.IsSuccessStatusCode)
                    return false;

                var json = await response.Content.ReadAsStringAsync();
                var resultado = JsonSerializer.Deserialize<JsonElement>(json, _jsonOptions);

                if (!resultado.TryGetProperty("Registros", out var registrosElement))
                    return false;

                var registros = registrosElement.EnumerateArray().ToList();
                return registros.Count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al verificar juego: {ex.Message}");
                return false;
            }
        }

        // ==================== HELPERS PARA JSON ====================

        private string ObtenerString(JsonElement elemento, string propiedad)
        {
            try
            {
                if (elemento.TryGetProperty(propiedad, out var prop) && prop.ValueKind != JsonValueKind.Null)
                    return prop.GetString() ?? "";
                return "";
            }
            catch
            {
                return "";
            }
        }

        private int ObtenerInt(JsonElement elemento, string propiedad)
        {
            try
            {
                if (elemento.TryGetProperty(propiedad, out var prop) && prop.ValueKind != JsonValueKind.Null)
                    return prop.GetInt32();
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private decimal ObtenerDecimal(JsonElement elemento, string propiedad)
        {
            try
            {
                if (elemento.TryGetProperty(propiedad, out var prop) && prop.ValueKind != JsonValueKind.Null)
                    return prop.GetDecimal();
                return 0m;
            }
            catch
            {
                return 0m;
            }
        }

        private decimal? ObtenerDecimalNullable(JsonElement elemento, string propiedad)
        {
            try
            {
                if (elemento.TryGetProperty(propiedad, out var prop) && prop.ValueKind != JsonValueKind.Null)
                    return prop.GetDecimal();
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}