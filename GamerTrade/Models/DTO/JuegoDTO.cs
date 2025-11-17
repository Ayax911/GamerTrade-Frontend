namespace GamerTrade.Models.DTO
{
    /// <summary>
    /// DTO básico para listar juegos en la tienda
    /// </summary>
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

        // ⭐ RENOMBRADO: Era UrlImagen, ahora ImagenURL (para que coincida con la BD)
        public string? UrlImagen { get; set; }

        // 🆕 NUEVO: Necesario para mostrar la categoría en las tarjetas
        public string? CategoriaNombre { get; set; }

        // Propiedades calculadas (formateadores)
        public string FechaFormateada => Fecha_lanzamiento?.ToString("dd/MM/yyyy") ?? "Por anunciar";
        public string CalificacionTexto => Calificacion.HasValue ? $"{Calificacion:F1} ⭐" : "Sin calificar";
        public string PrecioFormateado => $"${Precio:N2}";
    }

    /// <summary>
    /// DTO extendido con información completa del juego para la página de detalle
    /// </summary>
    public class JuegoDetalleDTO : JuegoDTO
    {
        // Información adicional de la categoría
        public string? CategoriaDescripcion { get; set; }

        // Información del desarrollador
        public string DesarrolladorNombre { get; set; } = string.Empty;

        // Estadísticas del juego
        public int CantidadDescargas { get; set; }
        public decimal TotalRecaudado { get; set; }

        // Reseñas del juego
        public List<ResenaDTO> Resenas { get; set; } = new();
    }

    /// <summary>
    /// DTO para las reseñas de un juego
    /// </summary>
    
}