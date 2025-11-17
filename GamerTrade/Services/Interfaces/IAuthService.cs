using GamerTrade.Models.DTO;

namespace GamerTrade.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Intenta autenticar al usuario con email y contraseña
        /// </summary>
        /// <returns>Tupla (éxito, mensaje)</returns>
        Task<(bool Success, string Message)> LoginAsync(LoginRequest request);

        /// <summary>
        /// Cierra la sesión del usuario actual
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// Obtiene los datos del usuario autenticado desde la BD
        /// </summary>
        Task<UsuarioDTO?> ObtenerUsuarioActualAsync();

        /// <summary>
        /// Verifica si hay un token válido en localStorage
        /// </summary>
        Task<bool> EstaAutenticadoAsync();

        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        Task<bool> RegistrarAsync(CrearUsuarioDTO request);
    }


}

