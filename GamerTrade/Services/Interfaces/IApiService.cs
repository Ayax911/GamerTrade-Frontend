namespace GamerTrade.Services.Interfaces
{
    public interface IApiService
    {
        Task<List<T>?> ListarAsync<T>(string tabla, string? esquema = "dbo", int? limite = null);
        Task<T?> ObtenerPorClaveAsync<T>(string tabla, string clave, string valor, string? esquema = "dbo");
        Task<bool> CrearAsync(string tabla, Dictionary<string, object?> datos, string? esquema = "dbo", string? camposEncriptar = null);
        Task<bool> ActualizarAsync(string tabla, string clave, string valor, Dictionary<string, object?> datos, string? esquema = "dbo", string? camposEncriptar = null);
        Task<bool> EliminarAsync(string tabla, string clave, string valor, string? esquema = "dbo");
    }
}
