namespace GamerTrade.Models.DTO
{
    public class UsuarioDTO
    {
        public int UsuarioID { get; set; }
        public string Apodo { get; set; } = string.Empty;
        public int RolID { get; set; }
        public decimal Saldo { get; set; }
        public string Correo { get; set; } = string.Empty;
        
    }
}
