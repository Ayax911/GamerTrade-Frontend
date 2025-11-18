using System.ComponentModel.DataAnnotations;

namespace GamerTrade.Models.DTO
{
    public class CrearUsuarioDTO
    {

        [Required(ErrorMessage = "El apodo es requerido")]
        [MinLength(3, ErrorMessage = "Mínimo 3 caracteres")]
        public string Apodo { get; set; } 

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Formato inválido")]
        public string Correo { get; set; } 

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public string Contraseña { get; set; } 

        public int RolID { get; set; } 
        public decimal Saldo { get; set; }
        
    }
}
