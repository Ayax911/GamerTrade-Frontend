using System.Net.Http.Json;
using System.Text.Json;
using GamerTrade.Services.Interfaces;

namespace GamerTrade.Services
{
    
    
    public class ApiService : IApiService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(HttpClient http)
        {
            _http = http;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        /// Lista todos los registros de una tabla
        /// </summary>
        public async Task<List<T>?> ListarAsync<T>(string tabla, string? esquema = "dbo", int? limite = null)
        {
            try
            {
                var url = $"api/Crud/listar?tabla={tabla}&esquema={esquema ?? "dbo"}";
                if (limite.HasValue)
                    url += $"&limite={limite}";

                Console.WriteLine($"🔵 GET: {url}");

                var resultado = await _http.GetFromJsonAsync<List<T>>(url, _jsonOptions);

                Console.WriteLine($"✅ Obtenidos {resultado?.Count ?? 0} registros de {tabla}");
                return resultado;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ Error HTTP en ListarAsync({tabla}): {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ListarAsync({tabla}): {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene UN registro por su clave primaria o única
        /// </summary>
        public async Task<T?> ObtenerPorClaveAsync<T>(string tabla, string clave, string valor, string? esquema = "dbo")
        {
            try
            {
                var valorEscapado = Uri.EscapeDataString(valor);
                var url = $"api/Crud/obtener?tabla={tabla}&esquema={esquema ?? "dbo"}&clave={clave}&valor={valorEscapado}";

                Console.WriteLine($"🔵 GET: {url}");

                var lista = await _http.GetFromJsonAsync<List<T>>(url, _jsonOptions);
                var resultado = lista.FirstOrDefault();

                if (resultado != null)
                    Console.WriteLine($"✅ Registro encontrado en {tabla}");
                else
                    Console.WriteLine($"⚠️ No se encontró registro en {tabla} con {clave}={valor}");

                return resultado;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ Error HTTP en ObtenerPorClaveAsync({tabla}): {ex.Message}");
                return default;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ObtenerPorClaveAsync({tabla}, {clave}={valor}): {ex.Message}");
                return default;
            }
        }

        /// <summary>
        /// Crea un nuevo registro en la tabla
        /// </summary>
        public async Task<bool> CrearAsync(string tabla, Dictionary<string, object?> datos, string? esquema = "dbo", string? camposEncriptar = null)
        {
            try
            {
                var url = $"api/Crud/crear?tabla={tabla}&esquema={esquema ?? "dbo"}";
                if (!string.IsNullOrEmpty(camposEncriptar))
                    url += $"&camposEncriptar={camposEncriptar}";

                Console.WriteLine($"🔵 POST: {url}");
                Console.WriteLine($"📤 Datos: {System.Text.Json.JsonSerializer.Serialize(datos)}");

                var response = await _http.PostAsJsonAsync(url, datos);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Registro creado en {tabla}");
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error al crear en {tabla}: {response.StatusCode} - {error}");
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ Error HTTP en CrearAsync({tabla}): {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en CrearAsync({tabla}): {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Actualiza un registro existente
        /// </summary>
        public async Task<bool> ActualizarAsync(string tabla, string clave, string valor, Dictionary<string, object?> datos, string? esquema = "dbo", string? camposEncriptar = null)
        {
            try
            {
                var valorEscapado = Uri.EscapeDataString(valor);
                var url = $"api/Crud/actualizar?tabla={tabla}&esquema={esquema ?? "dbo"}&clave={clave}&valor={valorEscapado}";

                if (!string.IsNullOrEmpty(camposEncriptar))
                    url += $"&camposEncriptar={camposEncriptar}";

                Console.WriteLine($"🔵 PUT: {url}");
                Console.WriteLine($"📤 Datos: {System.Text.Json.JsonSerializer.Serialize(datos)}");

                var response = await _http.PutAsJsonAsync(url, datos);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Registro actualizado en {tabla}");
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error al actualizar en {tabla}: {response.StatusCode} - {error}");
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ Error HTTP en ActualizarAsync({tabla}): {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ActualizarAsync({tabla}): {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un registro
        /// </summary>
        public async Task<bool> EliminarAsync(string tabla, string clave, string valor, string? esquema = "dbo")
        {
            try
            {
                var valorEscapado = Uri.EscapeDataString(valor);
                var url = $"api/Crud/eliminar?tabla={tabla}&esquema={esquema ?? "dbo"}&clave={clave}&valor={valorEscapado}";

                Console.WriteLine($"🔵 DELETE: {url}");

                var response = await _http.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Registro eliminado de {tabla}");
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error al eliminar de {tabla}: {response.StatusCode} - {error}");
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ Error HTTP en EliminarAsync({tabla}): {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en EliminarAsync({tabla}): {ex.Message}");
                return false;
            }
        }
    }
}