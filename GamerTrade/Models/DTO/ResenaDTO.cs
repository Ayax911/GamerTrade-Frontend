namespace GamerTrade.Models.DTO
{
    public class ResenaDTO
    {
        public int ResenaID { get; set; }
        public string? Descripcion { get; set; }
        public int Calificacion { get; set; }
        public DateTime Fecha { get; set; }
        public int JuegoID { get; set; }
        public int UsuarioID { get; set; }

        // ⭐ RENOMBRADO: Era UsuarioApodo, ahora NombreUsuario (para consistencia)
        public string NombreUsuario { get; set; } = string.Empty;

        // Propiedad calculada para fecha formateada
        public string FechaFormateada => Fecha.ToString("dd/MM/yyyy");
    }
}
