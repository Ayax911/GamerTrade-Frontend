using System.ComponentModel.DataAnnotations;

namespace GamerTrade.Models.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "El apodo es requerido")]
        [MinLength(3, ErrorMessage = "Mínimo 3 caracteres")]
        public string Apodo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirma tu contraseña")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
