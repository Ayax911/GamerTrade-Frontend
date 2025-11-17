namespace GamerTrade.Models.DTO
{
    public class ItemCarritoDTO
    {
        public int JuegoId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; } = 1;

        // Campos adicionales para la UI
        public string? ImagenURL { get; set; }
        public string? CategoriaNombre { get; set; }
        public decimal? Calificacion { get; set; }
        public Guid CarritoID { get; set; }
        public String UrlImagen { get; set; }

        // Propiedad calculada
        public decimal Subtotal => Precio * Cantidad;
    }
}
