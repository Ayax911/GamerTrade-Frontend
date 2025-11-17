namespace GamerTrade.Models.DTO
{
    public class JuegoDTO
    {
         public int JuegoID { get; set; }
         public string Titulo { get; set; } = string.Empty;
         public string? Versión { get; set; }
         public DateTime? Fecha_lanzamiento { get; set; }
         public decimal? Calificacion { get; set; }
         public decimal Precio { get; set; }
         public int CategoriaID { get; set; }
         public int DesarrolladorID { get; set; }
        public string Url { get; set; } 








        public string FechaFormateada => Fecha_lanzamiento?.ToString("dd/MM/yyyy") ?? "Por anunciar";
        public string CalificacionTexto => Calificacion.HasValue ? $"{Calificacion:F1} ⭐" : "Sin calificar";
        public string PrecioFormateado => $"${Precio:N2}";
    }

    public class JuegoDetalleDTO : JuegoDTO
    {
        public int CantidadDescargas { get; set; }
        public decimal TotalRecaudado { get; set; }
        public List<ResenaDTO> Reseñas { get; set; } = new();
    }

    public class ResenaDTO
    {
        public int ResenaID { get; set; }
        public string UsuarioApodo { get; set; }
        public int Calificacion { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
    }

}