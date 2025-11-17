namespace GamerTrade.Models.DTO
{
    public class ItemCarritoDTO
    {
        public int JuegoID { get; set; }
        public string Titulo { get; set; }
        public string UrlImagen { get; set; }
        public decimal Precio { get; set; }
        public DateTime FechaAgregado { get; set; }
    }
}
